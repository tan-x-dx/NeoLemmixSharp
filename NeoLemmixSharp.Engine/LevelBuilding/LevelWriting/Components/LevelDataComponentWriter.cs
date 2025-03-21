﻿using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.Components;

public sealed class LevelDataComponentWriter : ILevelDataWriter
{
    private const int NumberOfBytesForMainLevelData = 31;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelDataComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelDataSectionIdentifier;

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return 1;
    }

    public void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        writer.Write(GetNumberOfBytesWrittenForLevelData(levelData));

        WriteLevelStringData(writer, levelData);
        writer.Write(levelData.LevelId);
        writer.Write(levelData.Version);

        WriteLevelDimensionData(writer, levelData);
        WriteLevelBackgroundData(writer, levelData);
    }

    private static ushort GetNumberOfBytesWrittenForLevelData(LevelData levelData)
    {
        int numberOfBytesWrittenForBackgroundData;
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
        {
            numberOfBytesWrittenForBackgroundData = 1;
        }
        else if (backgroundData.IsSolidColor)
        {
            numberOfBytesWrittenForBackgroundData = 4;
        }
        else
        {
            numberOfBytesWrittenForBackgroundData = 3;
        }

        return (ushort)(NumberOfBytesForMainLevelData + numberOfBytesWrittenForBackgroundData);
    }

    private void WriteLevelStringData(
        BinaryWriter writer,
        LevelData levelData)
    {
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelTitle));
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelAuthor));
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelTheme));
    }

    private static void WriteLevelDimensionData(BinaryWriter writer, LevelData levelData)
    {
        writer.Write((ushort)levelData.LevelWidth);
        writer.Write((ushort)levelData.LevelHeight);
        writer.Write((ushort)(levelData.LevelStartPositionX ?? LevelReadWriteHelpers.UnspecifiedLevelStartValue));
        writer.Write((ushort)(levelData.LevelStartPositionY ?? LevelReadWriteHelpers.UnspecifiedLevelStartValue));

        var boundaryByte = GetBoundaryBehaviourByte(levelData);
        writer.Write(boundaryByte);
    }

    private static byte GetBoundaryBehaviourByte(LevelData levelData)
    {
        var horizontalData = (int)levelData.HorizontalBoundaryBehaviour;
        var verticalData = (int)levelData.VerticalBoundaryBehaviour;
        var combined = (verticalData << 1) | horizontalData;

        return (byte)combined;
    }

    private void WriteLevelBackgroundData(
        BinaryWriter writer,
        LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
        {
            writer.Write((byte)LevelReadWriteHelpers.NoBackgroundSpecified);

            return;
        }

        if (backgroundData.IsSolidColor)
        {
            writer.Write((byte)LevelReadWriteHelpers.SolidColorBackground);
            var actualBackgroundColor = backgroundData.Color;
            writer.Write(actualBackgroundColor.R);
            writer.Write(actualBackgroundColor.G);
            writer.Write(actualBackgroundColor.B);

            return;
        }

        writer.Write((byte)LevelReadWriteHelpers.TextureBackground);
        var backgroundStringId = _stringIdLookup[backgroundData.BackgroundImageName];
        writer.Write(backgroundStringId);
    }
}