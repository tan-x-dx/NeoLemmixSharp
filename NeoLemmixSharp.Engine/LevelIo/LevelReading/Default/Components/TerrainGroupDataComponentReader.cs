using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Components;

public sealed class TerrainGroupDataComponentReader : LevelDataComponentReader
{
    private readonly List<string> _stringIdLookup;
    private readonly TerrainDataComponentReader _terrainDataComponentReader;

    public TerrainGroupDataComponentReader(
        Version version,
        List<string> stringIdLookup,
        TerrainDataComponentReader terrainDataComponentReader)
        : base(LevelReadWriteHelpers.TerrainGroupDataSectionIdentifierIndex)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentReader = terrainDataComponentReader;
    }


    public override void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}