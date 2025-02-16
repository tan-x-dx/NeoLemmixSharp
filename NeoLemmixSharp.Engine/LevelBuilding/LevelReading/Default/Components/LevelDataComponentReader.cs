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
        LevelReadWriteHelpers.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level data item!");

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
        levelData.LevelWidth = rawFileData.Read16BitUnsignedInteger();
        levelData.LevelHeight = rawFileData.Read16BitUnsignedInteger();

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

        byte boundaryByte = rawFileData.Read8BitUnsignedInteger();

        DecipherBoundaryBehaviours(levelData, boundaryByte);
    }

    private static void DecipherBoundaryBehaviours(LevelData levelData, byte boundaryByte)
    {
        int intValue = boundaryByte;
        var horizontalBoundaryBehaviour = (BoundaryBehaviourType)(intValue & 1);
        var verticalBoundaryBehaviour = (BoundaryBehaviourType)((intValue >> 1) & 1);

        levelData.HorizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        levelData.VerticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    private void ReadBackgroundData(RawFileData rawFileData, LevelData levelData)
    {
        byte specifierByte = rawFileData.Read8BitUnsignedInteger();

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
            var buffer = rawFileData.ReadBytes(3);

            return new BackgroundData
            {
                IsSolidColor = true,
                Color = new Color(r: buffer[0], g: buffer[1], b: buffer[2], alpha: (byte)0xff),
                BackgroundImageName = string.Empty
            };
        }

        BackgroundData ReadTextureBackgroundData()
        {
            int backgroundStringId = rawFileData.Read16BitUnsignedInteger();

            return new BackgroundData
            {
                IsSolidColor = false,
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