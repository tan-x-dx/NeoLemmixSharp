using NeoLemmixSharp.Engine.LevelIo.Data;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Components;

public sealed class StringDataComponentReader : LevelDataComponentReader
{
    private readonly List<string> _stringIdLookup;

    public StringDataComponentReader(
        Version version,
        List<string> stringIdLookup)
        : base(LevelReadWriteHelpers.StringDataSectionIdentifierIndex)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItems = rawFileData.Read16BitUnsignedInteger();

        LevelReadingException.ReaderAssert(_stringIdLookup.Count == 0, "Expected string list to be empty!");
        _stringIdLookup.Capacity = numberOfItems + 1;
        _stringIdLookup.Add(string.Empty);

        while (numberOfItems-- > 0)
        {
            var actualString = ReadString(rawFileData);

            _stringIdLookup.Add(actualString);
        }
    }

    private string ReadString(RawFileData rawFileData)
    {
        int id = rawFileData.Read16BitUnsignedInteger();
        LevelReadingException.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

        // The next 16bit int specifies how many bytes make up the next string
        int stringLengthInBytes = rawFileData.Read16BitUnsignedInteger();
        var stringBytes = rawFileData.ReadBytes(stringLengthInBytes);
        var actualString = Encoding.UTF8.GetString(stringBytes);

        return actualString;
    }
}