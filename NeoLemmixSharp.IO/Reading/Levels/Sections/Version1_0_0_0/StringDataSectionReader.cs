using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class StringDataSectionReader : LevelDataSectionReader
{
    private readonly FileStringReader<LevelFileSectionIdentifierHasher, LevelFileSectionIdentifier> _stringReader;

    public StringDataSectionReader(
        StringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.StringDataSection, true)
    {
        _stringReader = new FileStringReader<LevelFileSectionIdentifierHasher, LevelFileSectionIdentifier>(stringIdLookup);
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        _stringReader.ReadSection(rawFileData);
    }
}