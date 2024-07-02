using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class TerrainGroupComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;
    private readonly TerrainComponentReader _terrainComponentReaderWriter;

    public TerrainGroupComponentReader(List<string> stringIdLookup, TerrainComponentReader terrainComponentReaderWriter)
    {
        _stringIdLookup = stringIdLookup;
        _terrainComponentReaderWriter = terrainComponentReaderWriter;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x7C, 0x5C];
        return sectionIdentifier;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}