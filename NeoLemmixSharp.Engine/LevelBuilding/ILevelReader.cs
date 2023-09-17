using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Io.LevelReading;

public interface ILevelReader : IDisposable
{
    LevelData LevelData { get; }

    void ReadLevel(string levelFilePath);
}