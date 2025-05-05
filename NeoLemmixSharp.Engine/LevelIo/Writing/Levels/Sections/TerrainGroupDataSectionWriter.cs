using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;

public sealed class TerrainGroupDataSectionWriter : LevelDataSectionWriter
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.TerrainGroupDataSection;
    public override bool IsNecessary => false;

    private readonly Dictionary<string, ushort> _stringIdLookup;
    private readonly TerrainDataSectionWriter _terrainDataComponentWriter;

    public TerrainGroupDataSectionWriter(
        Dictionary<string, ushort> stringIdLookup,
        TerrainDataSectionWriter terrainDataComponentWriter)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentWriter = terrainDataComponentWriter;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainGroups.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            WriteTerrainGroupData(writer, levelData.TerrainArchetypeData, terrainGroup);
        }
    }

    private void WriteTerrainGroupData(
        RawLevelFileDataWriter writer,
        Dictionary<StylePiecePair, TerrainArchetypeData> terrainArchetypeDataLookup,
        TerrainGroupData terrainGroupData)
    {
        writer.Write(_stringIdLookup[terrainGroupData.GroupName!]);
        writer.Write((ushort)terrainGroupData.AllBasicTerrainData.Count);

        foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
        {
            var terrainArchetypeData = terrainArchetypeDataLookup[terrainData.GetStylePiecePair()];

            _terrainDataComponentWriter.WriteTerrainData(writer, terrainArchetypeData, terrainData);
        }
    }
}