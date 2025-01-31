using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class TerrainGroupDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;
    private readonly TerrainDataComponentReader _terrainDataComponentReader;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.TerrainGroupDataSectionIdentifier;

    public TerrainGroupDataComponentReader(
        Version version, 
        List<string> stringIdLookup,
        TerrainDataComponentReader terrainDataComponentReader)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentReader = terrainDataComponentReader;
    }


    public void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}