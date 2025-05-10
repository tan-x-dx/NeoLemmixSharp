using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections.Version1_0_0_0;

public sealed class LevelObjectiveDataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.LevelObjectivesDataSection;
    public override bool IsNecessary => true;

    private readonly List<string> _stringIdLookup;

    public LevelObjectiveDataSectionReader(
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}