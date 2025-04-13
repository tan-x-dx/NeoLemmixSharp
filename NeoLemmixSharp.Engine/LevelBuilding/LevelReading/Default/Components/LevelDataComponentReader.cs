using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class LevelDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelDataSectionIdentifier;

    public LevelDataComponentReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(RawFileData rawFileData, LevelData levelData)
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
        uint specifierByte = rawFileData.Read8BitUnsignedInteger();

        levelData.LevelBackground = specifierByte switch
        {
            LevelReadWriteHelpers.NoBackgroundSpecified => null,
            LevelReadWriteHelpers.SolidColorBackground => ReadSolidColorBackgroundData(),
            LevelReadWriteHelpers.TextureBackground => ReadTextureBackgroundData(),

            _ => throw new LevelReadingException($"Unknown background specifier byte: {specifierByte:X}")
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