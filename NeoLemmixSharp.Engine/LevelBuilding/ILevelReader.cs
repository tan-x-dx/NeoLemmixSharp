using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public interface ILevelReader : IDisposable
{
    LevelData LevelData { get; }

    void ReadLevel(string levelFilePath);
}