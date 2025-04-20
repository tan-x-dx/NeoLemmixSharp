using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class StringDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.StringDataSectionIdentifier;

    public StringDataComponentReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItems = rawFileData.Read16BitUnsignedInteger();

        LevelReadingException.ReaderAssert(_stringIdLookup.Count == 0, "Expected string list to be empty!");
        _stringIdLookup.Capacity = numberOfItems + 1;
        _stringIdLookup.Add(string.Empty);

        var utf8Encoding = Encoding.UTF8;

        while (numberOfItems-- > 0)
        {
            int id = rawFileData.Read16BitUnsignedInteger();
            LevelReadingException.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

            // The next 16bit int specifies how many bytes make up the next string
            int stringLengthInBytes = rawFileData.Read16BitUnsignedInteger();
            var stringBytes = rawFileData.ReadBytes(stringLengthInBytes);
            var actualString = utf8Encoding.GetString(stringBytes);

            _stringIdLookup.Add(actualString);
        }
    }
}