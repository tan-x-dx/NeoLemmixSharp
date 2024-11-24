using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class DefaultLevelReader : ILevelReader
{
    public LevelData ReadLevel(string levelFilePath, GraphicsDevice graphicsDevice)
    {
        using var dataReaderList = new DataReaderList(levelFilePath);

        var levelData = dataReaderList.ReadFile();
        levelData.MaxNumberOfClonedLemmings = LevelReadingHelpers.CalculateMaxNumberOfClonedLemmings(levelData);

        return levelData;
    }

    public void Dispose()
    {
    }
}