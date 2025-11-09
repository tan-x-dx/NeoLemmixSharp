using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class LevelMetadataSectionWriter : LevelDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public LevelMetadataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelMetadataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return 1;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        WriteLevelStringData(writer, levelData);
        writer.Write64BitUnsignedInteger(levelData.LevelId.LevelId);
        writer.Write64BitUnsignedInteger(levelData.Version.Version);

        WriteLevelDimensionData(writer, levelData);
        WriteLevelBackgroundData(writer, levelData);
    }

    private void WriteLevelStringData(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(levelData.LevelTitle));
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(levelData.LevelAuthor));
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(levelData.LevelTheme));
    }

    private static void WriteLevelDimensionData(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        var levelDimensions = levelData.LevelDimensions;
        writer.Write16BitUnsignedInteger((ushort)levelDimensions.W);
        writer.Write16BitUnsignedInteger((ushort)levelDimensions.H);
        writer.Write16BitUnsignedInteger((ushort)(levelData.LevelStartPositionX ?? ReadWriteHelpers.UnspecifiedLevelStartValue));
        writer.Write16BitUnsignedInteger((ushort)(levelData.LevelStartPositionY ?? ReadWriteHelpers.UnspecifiedLevelStartValue));

        var boundaryByte = GetBoundaryBehaviourByte(levelData);
        writer.Write8BitUnsignedInteger(boundaryByte);
    }

    private static byte GetBoundaryBehaviourByte(LevelData levelData)
    {
        var horizontalData = (int)levelData.HorizontalBoundaryBehaviour;
        var verticalData = (int)levelData.VerticalBoundaryBehaviour;
        var combined = (verticalData << 1) | horizontalData;

        return (byte)combined;
    }

    private void WriteLevelBackgroundData(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;

        // Ensure that 5 bytes are always written, even if fewer bytes are utilized
        Span<byte> rawBytes = [0, 0, 0, 0, 0];

        if (backgroundData is null)
        {
            rawBytes[0] = (byte)BackgroundType.NoBackgroundSpecified;
        }
        else if (backgroundData.IsSolidColor)
        {
            rawBytes[0] = (byte)BackgroundType.SolidColorBackground;
            ReadWriteHelpers.WriteArgbBytes(backgroundData.Color, rawBytes[1..]);
        }
        else
        {
            rawBytes[0] = (byte)BackgroundType.TextureBackground;
            ushort backgroundStringId = _stringIdLookup.GetStringId(backgroundData.BackgroundImageName);
            Unsafe.WriteUnaligned(ref rawBytes[1], backgroundStringId);
        }

        writer.WriteBytes(rawBytes);
    }
}
