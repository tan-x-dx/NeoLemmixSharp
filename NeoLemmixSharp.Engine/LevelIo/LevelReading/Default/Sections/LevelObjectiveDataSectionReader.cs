using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public sealed class LevelObjectiveDataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.LevelObjectivesDataSection;
    public override bool IsNecessary => true;

    private readonly List<string> _stringIdLookup;

    public LevelObjectiveDataSectionReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileData rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}