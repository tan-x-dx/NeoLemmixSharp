using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections.Version1_0_0_0;

public sealed class LevelObjectiveDataSectionReader : LevelDataSectionReader
{
    public override bool IsNecessary => true;

    private readonly List<string> _stringIdLookup;

    public LevelObjectiveDataSectionReader(
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelObjectivesDataSection)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}