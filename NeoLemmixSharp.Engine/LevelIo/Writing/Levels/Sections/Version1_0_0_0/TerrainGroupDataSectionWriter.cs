using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections.Version1_0_0_0;

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
        throw new NotImplementedException();
    }
}