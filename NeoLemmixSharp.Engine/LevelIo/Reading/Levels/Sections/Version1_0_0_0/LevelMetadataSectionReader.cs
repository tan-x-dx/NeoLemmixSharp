using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections.Version1_0_0_0;

public sealed class LevelMetadataSectionReader : LevelDataSectionReader
{
    private const int NumberOfBytesWrittenForBackgroundData =
        1 + // Enum specifier
        4; // Four bytes for actual data, padding with zeros where necessary

    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.LevelMetadataSection;
    public override bool IsNecessary => true;

    private readonly List<string> _stringIdLookup;

    public LevelMetadataSectionReader(
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        FileReadingException.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level data item!");

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

        FileReadingException.AssertBytesMakeSense(
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
        var rawBytes = rawFileData.ReadBytes(NumberOfBytesWrittenForBackgroundData);

        uint rawBackgroundType = rawBytes[0];
        var backgroundType = (BackgroundType)rawBackgroundType;

        levelData.LevelBackground = backgroundType switch
        {
            BackgroundType.NoBackgroundSpecified => ReadNoBackgroundData(rawBytes),
            BackgroundType.SolidColorBackground => ReadSolidColorBackgroundData(rawBytes),
            BackgroundType.TextureBackground => ReadTextureBackgroundData(rawBytes),

            _ => Helpers.ThrowUnknownEnumValueException<BackgroundType, BackgroundData>(backgroundType)
        };

        return;

        static BackgroundData? ReadNoBackgroundData(ReadOnlySpan<byte> rawBytes)
        {
            Debug.Assert(rawBytes.Length == 5);

            return null;
        }

        static BackgroundData ReadSolidColorBackgroundData(ReadOnlySpan<byte> rawBytes)
        {
            Debug.Assert(rawBytes.Length == 5);

            return new BackgroundData
            {
                Color = LevelReadWriteHelpers.ReadArgbBytes(rawBytes[1..]),
                BackgroundImageName = string.Empty
            };
        }

        BackgroundData ReadTextureBackgroundData(ReadOnlySpan<byte> rawBytes)
        {
            Debug.Assert(rawBytes.Length == 5);
            ushort backgroundStringId = Unsafe.ReadUnaligned<ushort>(in rawBytes[1]);

            return new BackgroundData
            {
                Color = Color.Black,
                BackgroundImageName = _stringIdLookup[backgroundStringId]
            };
        }
    }
}