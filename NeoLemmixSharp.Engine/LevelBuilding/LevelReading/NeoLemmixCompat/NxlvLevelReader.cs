using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.TerrainReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    public LevelData ReadLevel(string levelFilePath)
    {
        var levelData = new LevelData();

        var terrainArchetypes = new Dictionary<string, TerrainArchetypeData>();
        using var dataReaders = new DataReaderList();

        // NOTE: The order of the data readers is important!
        dataReaders.Add(new LevelDataReader());
        dataReaders.Add(new SkillSetReader(levelData.SkillSetData));
        dataReaders.Add(new TerrainGroupReader(terrainArchetypes));
        dataReaders.Add(new TerrainReader(terrainArchetypes));
        dataReaders.Add(new GadgetReader());
        dataReaders.Add(new LemmingReader(levelData.AllLemmingData));
        dataReaders.Add(new TextReader(levelData.PreTextLines, "$PRETEXT"));
        dataReaders.Add(new TextReader(levelData.PostTextLines, "$POSTTEXT"));
        dataReaders.Add(new SketchReader(levelData.AllSketchData));

        dataReaders.ReadFile(levelFilePath);

        dataReaders.ApplyToLevelData(levelData);

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
