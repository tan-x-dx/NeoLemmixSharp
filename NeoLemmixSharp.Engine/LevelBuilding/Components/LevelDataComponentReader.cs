using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class LevelDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public LevelDataComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelDataSectionIdentifier;

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        var numberOfItemsInSection = reader.Read16BitUnsignedInteger();
        LevelReadWriteHelpers.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level data item!");

        var numberOfBytesToRead = reader.Read16BitUnsignedInteger();
        var initialBytesRead = reader.BytesRead;

        var stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelTitle = _stringIdLookup[stringId];

        stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelAuthor = _stringIdLookup[stringId];

        stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelTheme = _stringIdLookup[stringId];

        levelData.LevelId = reader.Read64BitUnsignedInteger();
        levelData.Version = reader.Read64BitUnsignedInteger();

        ReadLevelDimensionData(reader, levelData);
        ReadBackgroundData(reader, levelData);

        LevelReadWriteHelpers.ReaderAssert(
            reader.BytesRead - initialBytesRead == numberOfBytesToRead,
            "Wrong number of bytes read for level data section! " +
            $"Expected: {numberOfBytesToRead}, Actual: {reader.BytesRead - initialBytesRead}");
    }

    private static void ReadLevelDimensionData(BinaryReaderWrapper reader, LevelData levelData)
    {
        levelData.LevelWidth = reader.Read16BitUnsignedInteger();
        levelData.LevelHeight = reader.Read16BitUnsignedInteger();

        var value = reader.Read16BitUnsignedInteger();
        if (value != LevelReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionX = value;
        }

        value = reader.Read16BitUnsignedInteger();
        if (value != LevelReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionY = value;
        }

        var boundaryByte = reader.Read8BitUnsignedInteger();

        DecipherBoundaryBehaviours(levelData, boundaryByte);
    }

    private static void DecipherBoundaryBehaviours(LevelData levelData, byte boundaryByte)
    {
        var horizontalBoundaryBehaviour = (BoundaryBehaviourType)(boundaryByte & 1);
        var verticalBoundaryBehaviour = (BoundaryBehaviourType)((boundaryByte >> 1) & 1);

        levelData.HorizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        levelData.VerticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    [SkipLocalsInit]
    private void ReadBackgroundData(BinaryReaderWrapper reader, LevelData levelData)
    {
        var specifierByte = reader.Read8BitUnsignedInteger();

        switch (specifierByte)
        {
            case LevelReadWriteHelpers.NoBackgroundSpecified:
                levelData.LevelBackground = null;
                break;

            case LevelReadWriteHelpers.SolidBackgroundColor:
                Span<byte> buffer = stackalloc byte[3];
                reader.ReadBytes(buffer);

                levelData.LevelBackground = new BackgroundData
                {
                    IsSolidColor = true,
                    Color = new Color(buffer[0], buffer[1], buffer[2]),
                    BackgroundImageName = string.Empty
                };
                break;


            case LevelReadWriteHelpers.BackgroundImageSpecified:
                var backgroundStringId = reader.Read16BitUnsignedInteger();

                levelData.LevelBackground = new BackgroundData
                {
                    IsSolidColor = false,
                    Color = Color.Black,
                    BackgroundImageName = _stringIdLookup[backgroundStringId]
                };
                break;


            default:
                throw new LevelReadingException($"Unknown background specifier byte: {specifierByte:X}");
        }
    }
}