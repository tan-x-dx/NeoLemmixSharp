using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting.Components;

public sealed class TerrainGroupDataComponentWriter : LevelDataComponentWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;
    private readonly TerrainDataComponentWriter _terrainDataComponentWriter;

    public TerrainGroupDataComponentWriter(Dictionary<string, ushort> stringIdLookup, TerrainDataComponentWriter terrainDataComponentWriter)
        : base(LevelFileSectionIdentifier.TerrainGroupDataSection)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentWriter = terrainDataComponentWriter;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainGroups.Count;
    }

    public override void WriteSection(
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