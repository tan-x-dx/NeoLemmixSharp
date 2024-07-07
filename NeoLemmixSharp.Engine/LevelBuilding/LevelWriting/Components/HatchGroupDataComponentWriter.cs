using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.Components;

public sealed class HatchGroupDataComponentWriter : ILevelDataWriter
{
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.HatchGroupDataSectionIdentifier;

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllHatchGroupData.Count;
    }

    public void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        foreach (var hatchGroupData in levelData.AllHatchGroupData)
        {
            WriteHatchGroupData(writer, hatchGroupData);
        }
    }

    private static void WriteHatchGroupData(
        BinaryWriter writer,
        HatchGroupData hatchGroupData)
    {
        writer.Write((byte)hatchGroupData.HatchGroupId);
        writer.Write((byte)hatchGroupData.MinSpawnInterval);
        writer.Write((byte)hatchGroupData.MaxSpawnInterval);
        writer.Write((byte)hatchGroupData.InitialSpawnInterval);
    }
}