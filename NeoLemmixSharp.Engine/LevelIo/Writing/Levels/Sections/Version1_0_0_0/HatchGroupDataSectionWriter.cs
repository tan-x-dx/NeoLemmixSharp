using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections.Version1_0_0_0;

public sealed class HatchGroupDataSectionWriter : LevelDataSectionWriter
{
    public HatchGroupDataSectionWriter()
        : base(LevelFileSectionIdentifier.HatchGroupDataSection, false)
    {
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllHatchGroupData.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var hatchGroupData in levelData.AllHatchGroupData)
        {
            WriteHatchGroupData(writer, hatchGroupData);
        }
    }

    private static void WriteHatchGroupData(
        RawLevelFileDataWriter writer,
        HatchGroupData hatchGroupData)
    {
        writer.Write((byte)hatchGroupData.HatchGroupId);
        writer.Write((byte)hatchGroupData.MinSpawnInterval);
        writer.Write((byte)hatchGroupData.MaxSpawnInterval);
        writer.Write((byte)hatchGroupData.InitialSpawnInterval);
    }
}