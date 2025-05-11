using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

public sealed class TerrainGroupDataSectionWriter : LevelDataSectionWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;
    private readonly TerrainDataSectionWriter _terrainDataComponentWriter;

    public TerrainGroupDataSectionWriter(
        Dictionary<string, ushort> stringIdLookup,
        TerrainDataSectionWriter terrainDataComponentWriter)
        : base(LevelFileSectionIdentifier.TerrainGroupDataSection, false)
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