using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style;
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

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        FileReadingException.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level metadata item!");

        int stringId = rawFileData.Read16BitUnsignedInteger();
        levelData.LevelTitle = _stringIdLookup[stringId];

        stringId = rawFileData.Read16BitUnsignedInteger();
        levelData.LevelAuthor = _stringIdLookup[stringId];

        stringId = rawFileData.Read16BitUnsignedInteger();
        levelData.LevelTheme = new StyleIdentifier(_stringIdLookup[stringId]);

        levelData.LevelId = new LevelIdentifier(rawFileData.Read64BitUnsignedInteger());
        levelData.Version = new LevelVersion(rawFileData.Read64BitUnsignedInteger());

        ReadLevelDimensionData(rawFileData, levelData);
        ReadBackgroundData(rawFileData, levelData);
    }

    private static void ReadLevelDimensionData(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int levelWidth = rawFileData.Read16BitUnsignedInteger();
        int levelHeight = rawFileData.Read16BitUnsignedInteger();

        levelData.SetLevelWidth(levelWidth);
        levelData.SetLevelHeight(levelHeight);

        int value = rawFileData.Read16BitUnsignedInteger();
        if (value != ReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionX = value;
        }

        value = rawFileData.Read16BitUnsignedInteger();
        if (value != ReadWriteHelpers.UnspecifiedLevelStartValue)
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
        const int NumberOfBytesWrittenForBackgroundData =
            1 + // Enum specifier
            4; // Four bytes for actual data, padding with zeros where necessary

        var rawBytes = rawFileData.ReadBytes(NumberOfBytesWrittenForBackgroundData);

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
