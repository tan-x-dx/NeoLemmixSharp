using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class LevelMetadataSectionReader : LevelDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public LevelMetadataSectionReader(
        FileReaderStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelMetadataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        FileReadingException.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level metadata item!");

        int stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelTitle = _stringIdLookup[stringId];

        stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelAuthor = _stringIdLookup[stringId];

        stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelStyle = new StyleIdentifier(_stringIdLookup[stringId]);

        levelData.LevelId = new LevelIdentifier(reader.Read64BitUnsignedInteger());
        levelData.Version = new LevelVersion(reader.Read64BitUnsignedInteger());

        ReadLevelDimensionData(reader, levelData);
        ReadBackgroundData(reader, levelData);

        levelData.MaxNumberOfClonedLemmings = reader.Read16BitUnsignedInteger();
    }

    private static void ReadLevelDimensionData(RawLevelFileDataReader reader, LevelData levelData)
    {
        int levelWidth = reader.Read16BitUnsignedInteger();
        int levelHeight = reader.Read16BitUnsignedInteger();

        levelData.SetLevelWidth(levelWidth);
        levelData.SetLevelHeight(levelHeight);

        int value = reader.Read16BitUnsignedInteger();
        if (value != ReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionX = value;
        }

        value = reader.Read16BitUnsignedInteger();
        if (value != ReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionY = value;
        }

        uint boundaryByte = reader.Read8BitUnsignedInteger();

        DecipherBoundaryBehaviours(levelData, boundaryByte);
    }

    private static void DecipherBoundaryBehaviours(LevelData levelData, uint boundaryByteValue)
    {
        var horizontalBoundaryBehaviour = (BoundaryBehaviourType)(boundaryByteValue & 1U);
        var verticalBoundaryBehaviour = (BoundaryBehaviourType)((boundaryByteValue >>> 1) & 1U);

        levelData.HorizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        levelData.VerticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    private void ReadBackgroundData(RawLevelFileDataReader reader, LevelData levelData)
    {
        uint rawBackgroundType = reader.Read8BitUnsignedInteger();

        var backgroundType = BackgroundTypeHelpers.GetEnumValue(rawBackgroundType);

        var previousPosition = reader.Position; 

        levelData.LevelBackground = backgroundType switch
        {
            BackgroundType.NoBackgroundSpecified => ReadNoBackgroundData(),
            BackgroundType.SolidColorBackground => ReadSolidColorBackgroundData(),
            BackgroundType.TextureBackground => ReadTextureBackgroundData(),

            _ => Helpers.ThrowUnknownEnumValueException<BackgroundType, BackgroundData?>(backgroundType)
        };

        var positionAfterReadingBackgroundData = reader.Position;

        FileReadingException.ReaderAssert(positionAfterReadingBackgroundData - previousPosition == 4, "Need to read exactly FOUR bytes when processing background data!");

        return;

        // Aways read four bytes for actual data, discarding unnecessary bytes where relevant

        BackgroundData? ReadNoBackgroundData()
        {
            reader.ReadBytes(4);
            return null;
        }

        BackgroundData ReadSolidColorBackgroundData()
        {
            var color = reader.ReadArgbColor();
            return new BackgroundData(color);
        }

        BackgroundData ReadTextureBackgroundData()
        {
            int backgroundStringId = reader.Read16BitUnsignedInteger();
            reader.Read16BitUnsignedInteger();
            return new BackgroundData(_stringIdLookup[backgroundStringId]);
        }
    }
}
