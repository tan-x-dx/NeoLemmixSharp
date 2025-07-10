using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class HatchGroupDataSectionWriter : LevelDataSectionWriter
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
        writer.Write8BitUnsignedInteger((byte)hatchGroupData.HatchGroupId);
        writer.Write8BitUnsignedInteger((byte)hatchGroupData.MinSpawnInterval);
        writer.Write8BitUnsignedInteger((byte)hatchGroupData.MaxSpawnInterval);
        writer.Write8BitUnsignedInteger((byte)hatchGroupData.InitialSpawnInterval);
    }
}