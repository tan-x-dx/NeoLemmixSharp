using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting;

public interface ILevelDataWriter
{
    ReadOnlySpan<byte> GetSectionIdentifier();

    ushort CalculateNumberOfItemsInSection(LevelData levelData);

    void WriteSection(
        BinaryWriter writer,
        LevelData levelData);
}