using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public sealed class TerrainGroupDataSectionReader : LevelDataSectionReader
{
    private readonly List<string> _stringIdLookup;
    private readonly TerrainDataSectionReader _terrainDataComponentReader;

    public TerrainGroupDataSectionReader(
        Version version,
        List<string> stringIdLookup,
        TerrainDataSectionReader terrainDataComponentReader)
        : base(LevelFileSectionIdentifier.TerrainGroupDataSection)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentReader = terrainDataComponentReader;
    }


    public override void ReadSection(RawLevelFileData rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}