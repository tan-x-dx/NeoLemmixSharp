﻿using NeoLemmixSharp.Common;
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
        writer.Write((ushort)(lemmingData.Position.X + LevelReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(lemmingData.Position.Y + LevelReadWriteHelpers.PositionOffset));
        writer.Write(lemmingData.State);

        writer.Write((byte)DihedralTransformation.Encode(lemmingData.Orientation, lemmingData.FacingDirection));
        writer.Write((byte)lemmingData.TeamId);
        writer.Write((byte)lemmingData.InitialLemmingAction.Id);
    }
}