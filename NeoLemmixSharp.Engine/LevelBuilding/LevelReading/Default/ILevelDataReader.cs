using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public interface ILevelDataReader
{
    ReadOnlySpan<byte> GetSectionIdentifier();
    void ReadSection(BinaryReaderWrapper reader, LevelData levelData);
}