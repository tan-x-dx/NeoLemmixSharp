using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public interface ILevelReader : IDisposable
{
    LevelData ReadLevel(string levelFilePath);
}