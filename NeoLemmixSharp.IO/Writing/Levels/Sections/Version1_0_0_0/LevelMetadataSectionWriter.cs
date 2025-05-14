using NeoLemmixSharp.IO.Data.Level;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class LevelMetadataSectionWriter : LevelDataSectionWriter
{
    private const int NumberOfBytesForMainLevelData = 31;
    private const int NumberOfBytesWrittenForBackgroundData =
        1 + // Enum specifier
        4; // Four bytes for actual data, padding with zeros where necessary

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelMetadataSectionWriter(Dictionary<string, ushort> stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelMetadataSection, false)
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
        writer.Write(GetNumberOfBytesWrittenForLevelData(levelData));

        WriteLevelStringData(writer, levelData);
        writer.Write(levelData.LevelId);
        writer.Write(levelData.Version);

        WriteLevelDimensionData(writer, levelData);
        WriteLevelBackgroundData(writer, levelData);
    }

    private static ushort GetNumberOfBytesWrittenForLevelData(LevelData levelData)
    {
        return NumberOfBytesForMainLevelData + NumberOfBytesWrittenForBackgroundData;
    }

    private void WriteLevelStringData(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelTitle));
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelAuthor));
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelTheme));
    }

    private static void WriteLevelDimensionData(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        var levelDimensions = levelData.LevelDimensions;
        writer.Write((ushort)levelDimensions.W);
        writer.Write((ushort)levelDimensions.H);
        writer.Write((ushort)(levelData.LevelStartPositionX ?? ReadWriteHelpers.UnspecifiedLevelStartValue));
        writer.Write((ushort)(levelData.LevelStartPositionY ?? ReadWriteHelpers.UnspecifiedLevelStartValue));

        var boundaryByte = GetBoundaryBehaviourByte(levelData);
        writer.Write(boundaryByte);
    }

    private static byte GetBoundaryBehaviourByte(LevelData levelData)
    {
        var horizontalData = (int)levelData.HorizontalBoundaryBehaviour;
        var verticalData = (int)levelData.VerticalBoundaryBehaviour;
        var combined = verticalData << 1 | horizontalData;

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
            ushort backgroundStringId = _stringIdLookup[backgroundData.BackgroundImageName];
            Unsafe.WriteUnaligned(ref rawBytes[1], backgroundStringId);
        }

        writer.Write(rawBytes);
    }
}