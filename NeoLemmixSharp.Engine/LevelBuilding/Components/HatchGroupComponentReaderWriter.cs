using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelWriting;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class HatchGroupComponentReaderWriter : ILevelDataReader, ILevelDataWriter
{
    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x90, 0xD2];
        return sectionIdentifier;
    }

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