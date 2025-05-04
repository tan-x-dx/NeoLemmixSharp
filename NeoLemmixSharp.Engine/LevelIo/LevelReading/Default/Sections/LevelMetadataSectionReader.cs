using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public sealed class LevelMetadataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.LevelMetadataSection;
    public override bool IsNecessary => true;

    private readonly List<string> _stringIdLookup;

    public LevelMetadataSectionReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
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

        LevelReadingException.AssertBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead,
            "level data section");
    }

    private static void ReadLevelDimensionData(RawLevelFileDataReader rawFileData, LevelData levelData)
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

    private void ReadBackgroundData(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int rawBackgroundType = rawFileData.Read8BitUnsignedInteger();
        var backgroundType = BackgroundTypeHelpers.GetEnumValue(rawBackgroundType);

        levelData.LevelBackground = backgroundType switch
        {
            BackgroundType.NoBackgroundSpecified => ReadNoBackgroundData(),
            BackgroundType.SolidColorBackground => ReadSolidColorBackgroundData(),
            BackgroundType.TextureBackground => ReadTextureBackgroundData(),

            _ => Helpers.ThrowUnknownEnumValueException<BackgroundType, BackgroundData>(rawBackgroundType)
        };

        return;

        BackgroundData? ReadNoBackgroundData()
        {
            _ = rawFileData.Read32BitSignedInteger();
            return null;
        }

        BackgroundData ReadSolidColorBackgroundData()
        {
            var color = rawFileData.ReadArgbColor();

            return new BackgroundData
            {
                Color = color,
                BackgroundImageName = string.Empty
            };
        }

        BackgroundData ReadTextureBackgroundData()
        {
            int backgroundStringId = rawFileData.Read16BitUnsignedInteger();
            _ = rawFileData.Read16BitUnsignedInteger();

            return new BackgroundData
            {
                Color = Color.Black,
                BackgroundImageName = _stringIdLookup[backgroundStringId]
            };
        }
    }
}