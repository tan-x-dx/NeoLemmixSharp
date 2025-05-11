using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class TerrainGroupDataSectionReader : LevelDataSectionReader
{
    private readonly List<string> _stringIdLookup;
    private readonly TerrainDataSectionReader _terrainDataComponentReader;

    public TerrainGroupDataSectionReader(
        List<string> stringIdLookup,
        TerrainDataSectionReader terrainDataComponentReader)
        : base(LevelFileSectionIdentifier.TerrainGroupDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentReader = terrainDataComponentReader;
    }


    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}