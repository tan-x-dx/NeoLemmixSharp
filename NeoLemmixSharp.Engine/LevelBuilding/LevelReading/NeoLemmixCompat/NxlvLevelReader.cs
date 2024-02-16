using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.TerrainReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    public LevelData ReadLevel(string levelFilePath)
    {
        var levelData = new LevelData();

        var terrainArchetypes = new Dictionary<string, TerrainArchetypeData>();

        var terrainGroupReader = new TerrainGroupReader(terrainArchetypes);
        var gadgetReader = new GadgetReader();
        var lemmingReader = new LemmingReader(levelData.AllLemmingData);

        // NOTE: The order of the data readers is important!
        var dataReaders = new INeoLemmixDataReader[]
        {
            new LevelDataReader(levelData),
            new SkillSetReader(levelData.SkillSetData),
            terrainGroupReader,
            new TerrainReader(terrainArchetypes, levelData.AllTerrainData),
            gadgetReader,
            lemmingReader,
            new NeoLemmixTextReader(levelData.PreTextLines, "$PRETEXT"),
            new NeoLemmixTextReader(levelData.PostTextLines, "$POSTTEXT"),
            new SketchReader(levelData.AllSketchData),
        };

        var dataReaderList = new DataReaderList(dataReaders);

        dataReaderList.ReadFile(levelFilePath);

        terrainGroupReader.ApplyToLevelData(levelData);
        gadgetReader.ApplyToLevelData(levelData);
        lemmingReader.ApplyToLevelData(levelData);

        SetUpLevelData(levelData);

        return levelData;
    }

    private static void SetUpLevelData(LevelData levelData)
    {

    }

    public void Dispose()
    {
    }
}
