using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting.Components;

public sealed class PrePlacedLemmingDataComponentWriter : LevelDataComponentWriter
{
    public PrePlacedLemmingDataComponentWriter()
        : base(LevelFileSectionIdentifier.PrePlacedLemmingDataSection)
    {
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.PrePlacedLemmingData.Count;
    }

    public override void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        foreach (var lemmingData in levelData.PrePlacedLemmingData)
        {
            WriteLemmingData(writer, lemmingData);
        }
    }

    private static void WriteLemmingData(BinaryWriter writer, LemmingData lemmingData)
    {
        writer.Write((ushort)(lemmingData.Position.X + LevelReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(lemmingData.Position.Y + LevelReadWriteHelpers.PositionOffset));
        writer.Write(lemmingData.State);

        writer.Write((byte)DihedralTransformation.Encode(lemmingData.Orientation, lemmingData.FacingDirection));
        writer.Write((byte)lemmingData.TeamId);
        writer.Write((byte)lemmingData.InitialLemmingAction.Id);
    }
}