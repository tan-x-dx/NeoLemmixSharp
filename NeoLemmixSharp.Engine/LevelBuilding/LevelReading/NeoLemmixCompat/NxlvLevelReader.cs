using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    public LevelData ReadLevel(string levelFilePath, GraphicsDevice graphicsDevice)
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
        gadgetReader.ApplyToLevelData(levelData, graphicsDevice);
        lemmingReader.ApplyToLevelData(levelData);

        return levelData;
    }

    public void Dispose()
    {
    }
}
