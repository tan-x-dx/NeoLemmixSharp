using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public interface ILevelDataReader
{
    bool AlreadyUsed { get; }
    ReadOnlySpan<byte> GetSectionIdentifier();
    void ReadSection(BinaryReaderWrapper reader, LevelData levelData);
}