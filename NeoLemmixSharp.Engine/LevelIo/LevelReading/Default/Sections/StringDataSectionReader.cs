using NeoLemmixSharp.Engine.LevelIo.Data;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public sealed class StringDataSectionReader : LevelDataSectionReader
{
    private readonly List<string> _stringIdLookup;

    public StringDataSectionReader(
        Version version,
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.StringDataSection)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileData rawFileData, LevelData levelData)
    {
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

    private string ReadString(RawLevelFileData rawFileData)
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