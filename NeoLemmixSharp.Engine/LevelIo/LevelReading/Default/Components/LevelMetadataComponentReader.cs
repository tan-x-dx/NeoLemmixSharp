using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Components;

public sealed class LevelMetadataComponentReader : LevelDataComponentReader
{
    private readonly List<string> _stringIdLookup;

    public LevelMetadataComponentReader(
        Version version,
        List<string> stringIdLookup)
        : base(LevelReadWriteHelpers.LevelMetadataSectionIdentifierIndex)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        LevelReadingException.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level data item!");

        int numberOfBytesToRead = rawFileData.Read16BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        int stringId = rawFileData.Read16BitUnsignedInteger();
        levelData.LevelTitle = _stringIdLookup[stringId];

        stringId = rawFileData.Read16BitUnsignedInteger();
        levelData.LevelAuthor = _stringIdLookup[stringId];

        stringId = rawFileData.Read16BitUnsignedInteger();
        levelData.LevelTheme = _stringIdLookup[stringId];

        levelData.LevelId = rawFileData.Read64BitUnsignedInteger();
        levelData.Version = rawFileData.Read64BitUnsignedInteger();

        ReadLevelDimensionData(rawFileData, levelData);
        ReadBackgroundData(rawFileData, levelData);

        AssertLevelDataBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead);
    }

    private static void ReadLevelDimensionData(RawFileData rawFileData, LevelData levelData)
    {
        int levelWidth = rawFileData.Read16BitUnsignedInteger();
        int levelHeight = rawFileData.Read16BitUnsignedInteger();

        levelData.SetLevelWidth(levelWidth);
        levelData.SetLevelHeight(levelHeight);

        int value = rawFileData.Read16BitUnsignedInteger();
        if (value != LevelReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionX = value;
        }

        value = rawFileData.Read16BitUnsignedInteger();
        if (value != LevelReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionY = value;
        }

        uint boundaryByte = rawFileData.Read8BitUnsignedInteger();

        DecipherBoundaryBehaviours(levelData, boundaryByte);
    }

    private static void DecipherBoundaryBehaviours(LevelData levelData, uint boundaryByteValue)
    {
        var horizontalBoundaryBehaviour = (BoundaryBehaviourType)(boundaryByteValue & 1U);
        var verticalBoundaryBehaviour = (BoundaryBehaviourType)((boundaryByteValue >>> 1) & 1U);

        levelData.HorizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        levelData.VerticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    private void ReadBackgroundData(RawFileData rawFileData, LevelData levelData)
    {
        int rawBackgroundType = rawFileData.Read8BitUnsignedInteger();
        var backgroundType = BackgroundTypeHelpers.GetBackgroundType(rawBackgroundType);

        levelData.LevelBackground = backgroundType switch
        {
            BackgroundType.NoBackgroundSpecified => null,
            BackgroundType.SolidColorBackground => ReadSolidColorBackgroundData(),
            BackgroundType.TextureBackground => ReadTextureBackgroundData(),

            _ => Helpers.ThrowUnknownEnumValueException<BackgroundType, BackgroundData>(rawBackgroundType)
        };

        return;

        BackgroundData ReadSolidColorBackgroundData()
        {
            return new BackgroundData
            {
                Color = rawFileData.ReadRgbColor(),
                BackgroundImageName = string.Empty
            };
        }

        BackgroundData ReadTextureBackgroundData()
        {
            int backgroundStringId = rawFileData.Read16BitUnsignedInteger();

            return new BackgroundData
            {
                Color = Color.Black,
                BackgroundImageName = _stringIdLookup[backgroundStringId]
            };
        }
    }

    private static void AssertLevelDataBytesMakeSense(
        int currentPosition,
        int initialPosition,
        int expectedByteCount)
    {
        if (currentPosition - initialPosition == expectedByteCount)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for level data section! " +
            $"Expected: {expectedByteCount}, Actual: {currentPosition - initialPosition}");
    }
}