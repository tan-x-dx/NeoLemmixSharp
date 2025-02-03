using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.Components;

public sealed class PrePlacedLemmingDataComponentWriter : ILevelDataWriter
{
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.PrePlacedLemmingDataSectionIdentifier;

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.PrePlacedLemmingData.Count;
    }

    public void WriteSection(
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
        writer.Write((ushort)(lemmingData.X + LevelReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(lemmingData.Y + LevelReadWriteHelpers.PositionOffset));
        writer.Write(lemmingData.State);

        writer.Write(LevelReadWriteHelpers.GetOrientationByte(lemmingData.Orientation, lemmingData.FacingDirection));
        writer.Write((byte)lemmingData.TeamId);
        writer.Write((byte)lemmingData.InitialLemmingAction.Id);
    }
}