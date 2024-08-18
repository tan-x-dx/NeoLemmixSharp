using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class TerrainGroupDataComponentReader : ILevelDataReader
{
    private readonly Dictionary<ushort, string> _stringIdLookup;
    private readonly TerrainDataComponentReader _terrainDataComponentReader;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.TerrainGroupDataSectionIdentifier;

    public TerrainGroupDataComponentReader(Dictionary<ushort, string> stringIdLookup, TerrainDataComponentReader terrainDataComponentReader)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentReader = terrainDataComponentReader;
    }


    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}