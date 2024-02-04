using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.TerrainReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    public LevelData ReadLevel(string levelFilePath)
    {
        var terrainArchetypes = new Dictionary<string, TerrainArchetypeData>();
        using var dataReaders = new DataReaderList();

        // NOTE: The order of the data readers is important!
        dataReaders.Add(new LevelDataReader());
        dataReaders.Add(new SkillSetReader());
        dataReaders.Add(new TerrainGroupReader(terrainArchetypes));
        dataReaders.Add(new TerrainReader(terrainArchetypes));
        dataReaders.Add(new GadgetReader());
        dataReaders.Add(new LemmingReader());

        dataReaders.ReadFile(levelFilePath);

        return SetUpLevelData(dataReaders);
    }

    private static LevelData SetUpLevelData(DataReaderList dataReaders)
    {
        var levelData = new LevelData();

        dataReaders.ApplyToLevelData(levelData);

        return levelData;
    }

    public void Dispose()
    {
    }
}
