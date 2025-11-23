using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;
using System.Runtime.CompilerServices;

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
        levelData.LevelTheme = _stringIdLookup[stringId];

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
        const int NumberOfBytesWrittenForBackgroundData =
            1 + // Enum specifier
            4; // Four bytes for actual data, padding with zeros where necessary

        var rawBytes = reader.ReadBytes(NumberOfBytesWrittenForBackgroundData);

        int rawBackgroundType = rawBytes[0];
        var backgroundType = (BackgroundType)rawBackgroundType;

        levelData.LevelBackground = backgroundType switch
        {
            BackgroundType.NoBackgroundSpecified => ReadNoBackgroundData(rawBytes),
            BackgroundType.SolidColorBackground => ReadSolidColorBackgroundData(rawBytes),
            BackgroundType.TextureBackground => ReadTextureBackgroundData(rawBytes),

            _ => Helpers.ThrowUnknownEnumValueException<BackgroundType, BackgroundData?>(backgroundType)
        };

        return;

        static BackgroundData? ReadNoBackgroundData(ReadOnlySpan<byte> rawBytes)
        {
            return null;
        }

        static BackgroundData ReadSolidColorBackgroundData(ReadOnlySpan<byte> rawBytes)
        {
            return new BackgroundData(ReadWriteHelpers.ReadArgbBytes(rawBytes[1..]));
        }

        BackgroundData ReadTextureBackgroundData(ReadOnlySpan<byte> rawBytes)
        {
            ushort backgroundStringId = Unsafe.ReadUnaligned<ushort>(in rawBytes[1]);

            return new BackgroundData(_stringIdLookup[backgroundStringId]);
        }
    }
}
