using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;

public sealed class HatchGroupDataSectionWriter : LevelDataSectionWriter
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.HatchGroupDataSection;
    public override bool IsNecessary => false;

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