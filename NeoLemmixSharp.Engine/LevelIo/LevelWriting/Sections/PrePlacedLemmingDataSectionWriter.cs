using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting.Sections;

public sealed class PrePlacedLemmingDataSectionWriter : LevelDataSectionWriter
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.PrePlacedLemmingDataSection;
    public override bool IsNecessary => false;

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.PrePlacedLemmingData.Count;
    }

    public override void WriteSection(
        RawFileData writer,
        LevelData levelData)
    {
        foreach (var lemmingData in levelData.PrePlacedLemmingData)
        {
            WriteLemmingData(writer, lemmingData);
        }
    }

    private static void WriteLemmingData(
        RawFileData writer,
        LemmingData lemmingData)
    {
        writer.Write((ushort)(lemmingData.Position.X + LevelReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(lemmingData.Position.Y + LevelReadWriteHelpers.PositionOffset));
        writer.Write(lemmingData.State);

        writer.Write((byte)DihedralTransformation.Encode(lemmingData.Orientation, lemmingData.FacingDirection));
        writer.Write((byte)lemmingData.TeamId);
        writer.Write((byte)lemmingData.InitialLemmingAction.Id);
    }
}