using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Components;

public sealed class LevelObjectiveDataComponentReader : LevelDataComponentReader
{
    private readonly List<string> _stringIdLookup;

    public LevelObjectiveDataComponentReader(
        Version version,
        List<string> stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelObjectivesDataSection)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}