using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public sealed class TerrainGroupComponentWriter : ILevelDataWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;
    private readonly TerrainComponentWriter _terrainComponentReaderWriter;

    public TerrainGroupComponentWriter(Dictionary<string, ushort> stringIdLookup, TerrainComponentWriter terrainComponentReaderWriter)
    {
        _stringIdLookup = stringIdLookup;
        _terrainComponentReaderWriter = terrainComponentReaderWriter;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x7C, 0x5C];
        return sectionIdentifier;
    }

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainGroups.Count;
    }

    public void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            WriteTerrainGroupData(writer, levelData.TerrainArchetypeData, terrainGroup);
        }
    }

    private void WriteTerrainGroupData(
        BinaryWriter writer,
        List<TerrainArchetypeData> allTerrainArchetypeData,
        TerrainGroup terrainGroup)
    {
        writer.Write(_stringIdLookup[terrainGroup.GroupName!]);
        writer.Write(terrainGroup.TerrainDatas.Count);

        foreach (var terrainData in terrainGroup.TerrainDatas)
        {
            var terrainArchetypeData = allTerrainArchetypeData.Find(a => a.TerrainArchetypeId == terrainData.TerrainArchetypeId);
            if (terrainArchetypeData is null)
                throw new InvalidOperationException($"Could not locate TerrainArchetypeData with id {terrainData.TerrainArchetypeId}");

            _terrainComponentReaderWriter.WriteTerrainData(writer, terrainArchetypeData, terrainData);
        }
    }
}