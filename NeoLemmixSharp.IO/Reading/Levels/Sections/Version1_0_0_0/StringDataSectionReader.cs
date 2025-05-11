using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

public sealed class StringDataSectionReader : LevelDataSectionReader
{

    private readonly FileStringReader<LevelFileSectionIdentifierHasher, LevelFileSectionIdentifier> _stringReader;

    public StringDataSectionReader(
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.StringDataSection, true)
    {
        _stringReader = new FileStringReader<LevelFileSectionIdentifierHasher, LevelFileSectionIdentifier>(stringIdLookup);
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        _stringReader.ReadSection(rawFileData);
    }
}