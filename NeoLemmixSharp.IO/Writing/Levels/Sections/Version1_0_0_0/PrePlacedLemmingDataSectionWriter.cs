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
        foreach (var lemmingDatum in levelData.PrePlacedLemmingData)
        {
            WriteLemmingData(writer, lemmingDatum);
        }
    }

    private static void WriteLemmingData(
        RawLevelFileDataWriter writer,
        LemmingInstanceData lemmingDatum)
    {
        writer.Write32BitSignedInteger(ReadWriteHelpers.EncodePoint(lemmingDatum.Position));
        writer.Write32BitUnsignedInteger(lemmingDatum.State);

        writer.Write8BitUnsignedInteger((byte)DihedralTransformation.Encode(lemmingDatum.Orientation, lemmingDatum.FacingDirection));
        writer.Write8BitUnsignedInteger((byte)lemmingDatum.TribeId);
        writer.Write8BitUnsignedInteger((byte)lemmingDatum.InitialLemmingActionId);
    }
}
