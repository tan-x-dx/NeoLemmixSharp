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