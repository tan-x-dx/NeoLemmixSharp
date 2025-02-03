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

        _stringIdLookup.Add(string.Empty);
    }

    public void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItems = rawFileData.Read16BitUnsignedInteger();

        while (numberOfItems-- > 0)
        {
            int id = rawFileData.Read16BitUnsignedInteger();
            LevelReadWriteHelpers.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

            // The next 16bit int specifies how many bytes make up the next string
            int stringLengthInBytes = rawFileData.Read16BitUnsignedInteger();
            var stringBytes = rawFileData.ReadBytes(stringLengthInBytes);
            var actualString = Encoding.UTF8.GetString(stringBytes);

            _stringIdLookup.Add(actualString);
        }
    }
}