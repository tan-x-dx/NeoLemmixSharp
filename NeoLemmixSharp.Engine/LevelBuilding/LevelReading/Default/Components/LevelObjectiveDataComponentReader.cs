using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class LevelObjectiveDataComponentReader : LevelDataComponentReader
{
    private readonly List<string> _stringIdLookup;

    public LevelObjectiveDataComponentReader(
        Version version,
        List<string> stringIdLookup)
        : base(LevelReadWriteHelpers.LevelObjectivesDataSectionIdentifierIndex)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}