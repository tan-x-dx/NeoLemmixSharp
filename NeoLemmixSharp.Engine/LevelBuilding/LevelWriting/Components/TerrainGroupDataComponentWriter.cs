using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using static NeoLemmixSharp.Engine.LevelBuilding.Data.LevelData;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.Components;

public sealed class TerrainGroupDataComponentWriter : ILevelDataWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;
    private readonly TerrainDataComponentWriter _terrainDataComponentWriter;

    public TerrainGroupDataComponentWriter(Dictionary<string, ushort> stringIdLookup, TerrainDataComponentWriter terrainDataComponentWriter)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentWriter = terrainDataComponentWriter;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.TerrainGroupDataSectionIdentifier;

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
        Dictionary<StylePiecePair, TerrainArchetypeData> terrainArchetypeDataLookup,
        TerrainGroupData terrainGroupData)
    {
        writer.Write(_stringIdLookup[terrainGroupData.GroupName!]);
        writer.Write(terrainGroupData.AllBasicTerrainData.Count);

        foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
        {
            var terrainArchetypeData = terrainArchetypeDataLookup[terrainData.GetStylePiecePair()];

            _terrainDataComponentWriter.WriteTerrainData(writer, terrainArchetypeData, terrainData);
        }
    }
}