using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class PrePlacedLemmingDataSectionWriter : LevelDataSectionWriter
{
    public PrePlacedLemmingDataSectionWriter()
        : base(LevelFileSectionIdentifier.PrePlacedLemmingDataSection, false)
    {
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.PrePlacedLemmingData.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var lemmingData in levelData.PrePlacedLemmingData)
        {
            WriteLemmingData(writer, lemmingData);
        }
    }

    private static void WriteLemmingData(
        RawLevelFileDataWriter writer,
        LemmingData lemmingData)
    {
        writer.Write32BitSignedInteger(ReadWriteHelpers.EncodePoint(lemmingData.Position));
        writer.Write32BitUnsignedInteger(lemmingData.State);

        writer.Write8BitUnsignedInteger((byte)DihedralTransformation.Encode(lemmingData.Orientation, lemmingData.FacingDirection));
        writer.Write8BitUnsignedInteger((byte)lemmingData.TribeId);
        writer.Write8BitUnsignedInteger((byte)lemmingData.InitialLemmingActionId);
    }
}
