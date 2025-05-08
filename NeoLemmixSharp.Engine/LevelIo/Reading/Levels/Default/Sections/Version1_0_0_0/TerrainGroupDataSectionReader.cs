using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections.Version1_0_0_0;

public sealed class TerrainGroupDataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.TerrainGroupDataSection;
    public override bool IsNecessary => false;

    private readonly List<string> _stringIdLookup;
    private readonly TerrainDataSectionReader _terrainDataComponentReader;

    public TerrainGroupDataSectionReader(
        List<string> stringIdLookup,
        TerrainDataSectionReader terrainDataComponentReader)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentReader = terrainDataComponentReader;
    }


    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}