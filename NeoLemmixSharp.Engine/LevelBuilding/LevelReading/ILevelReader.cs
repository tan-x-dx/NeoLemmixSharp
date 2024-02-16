using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public interface ILevelReader : IDisposable
{
    LevelData ReadLevel(string levelFilePath);
}