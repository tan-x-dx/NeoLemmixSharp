using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class TerrainGroupDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;
    private readonly TerrainDataComponentReader _terrainDataComponentReader;

    public TerrainGroupDataComponentReader(List<string> stringIdLookup, TerrainDataComponentReader terrainDataComponentReader)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentReader = terrainDataComponentReader;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.TerrainGroupDataSectionIdentifier;

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}