using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections.Version1_0_0_0;

public sealed class StringDataSectionReader : LevelDataSectionReader
{
    public override bool IsNecessary => true;

    private readonly FileStringReader<LevelFileSectionIdentifierHasher, LevelFileSectionIdentifier> _stringReader;

    public StringDataSectionReader(
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.StringDataSection)
    {
        _stringReader = new FileStringReader<LevelFileSectionIdentifierHasher, LevelFileSectionIdentifier>(stringIdLookup);
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        _stringReader.ReadSection(rawFileData);
    }
}