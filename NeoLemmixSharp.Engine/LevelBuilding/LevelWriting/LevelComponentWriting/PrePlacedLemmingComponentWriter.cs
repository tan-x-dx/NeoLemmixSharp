using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public sealed class PrePlacedLemmingComponentWriter : ILevelDataWriter
{
    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0xFE, 0x77];
        return sectionIdentifier;
    }

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
        writer.Write((ushort)(lemmingData.X + Helpers.PositionOffset));
        writer.Write((ushort)(lemmingData.Y + Helpers.PositionOffset));
        writer.Write(lemmingData.State);

        writer.Write(Helpers.GetOrientationByte(lemmingData.Orientation, lemmingData.FacingDirection));
        writer.Write((byte)lemmingData.Team.Id);
        writer.Write((ushort)lemmingData.InitialLemmingAction.Id);
    }
}