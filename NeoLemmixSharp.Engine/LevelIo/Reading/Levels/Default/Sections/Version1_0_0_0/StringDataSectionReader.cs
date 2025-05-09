using NeoLemmixSharp.Engine.LevelIo.Data;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections.Version1_0_0_0;

public sealed class StringDataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.StringDataSection;
    public override bool IsNecessary => true;

    private readonly List<string> _stringIdLookup;

    public StringDataSectionReader(
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int numberOfItems = rawFileData.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(_stringIdLookup.Count == 0, "Expected string list to be empty!");
        _stringIdLookup.Capacity = numberOfItems + 1;
        _stringIdLookup.Add(string.Empty);

        while (numberOfItems-- > 0)
        {
            var actualString = ReadString(rawFileData);

            _stringIdLookup.Add(actualString);
        }
    }

    private string ReadString(RawLevelFileDataReader rawFileData)
    {
        int id = rawFileData.Read16BitUnsignedInteger();
        FileReadingException.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

        // The next 16bit int specifies how many bytes make up the next string
        int stringLengthInBytes = rawFileData.Read16BitUnsignedInteger();
        var stringBytes = rawFileData.ReadBytes(stringLengthInBytes);
        var actualString = Encoding.UTF8.GetString(stringBytes);

        return actualString;
    }
}