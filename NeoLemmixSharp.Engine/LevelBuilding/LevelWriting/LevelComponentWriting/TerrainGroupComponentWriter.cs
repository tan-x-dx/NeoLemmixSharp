using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public static class TerrainGroupComponentWriter
{
    private static ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x7C, 0x5C];
        return sectionIdentifier;
    }

    private static ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainGroups.Count;
    }

    public static void WriteSection(
        BinaryWriter writer,
        Dictionary<string, ushort> stringIdLookup,
        LevelData levelData)
    {
        writer.Write(GetSectionIdentifier());
        writer.Write(CalculateNumberOfItemsInSection(levelData));

        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            WriteTerrainGroupData(writer, stringIdLookup, levelData.TerrainArchetypeData, terrainGroup);
        }
    }

    private static void WriteTerrainGroupData(
        BinaryWriter writer,
        Dictionary<string, ushort> stringIdLookup,
        List<TerrainArchetypeData> allTerrainArchetypeData,
        TerrainGroup terrainGroup)
    {
        writer.Write(stringIdLookup[terrainGroup.GroupName!]);
        writer.Write(terrainGroup.TerrainDatas.Count);

        foreach (var terrainData in terrainGroup.TerrainDatas)
        {
            var terrainArchetypeData = allTerrainArchetypeData.Find(a => a.TerrainArchetypeId == terrainData.TerrainArchetypeId);
            if (terrainArchetypeData is null)
                throw new InvalidOperationException($"Could not locate TerrainArchetypeData with id {terrainData.TerrainArchetypeId}");

            TerrainComponentWriter.WriteTerrainData(writer, stringIdLookup, terrainArchetypeData, terrainData);
        }
    }
}