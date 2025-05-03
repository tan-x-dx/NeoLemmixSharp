using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting.Sections;

public sealed class LevelMetadataSectionWriter : LevelDataSectionWriter
{
    private const int NumberOfBytesForMainLevelData = 31;

    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.LevelMetadataSection;
    public override bool IsNecessary => true;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelMetadataSectionWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return 1;
    }

    public override void WriteSection(
        RawFileData writer,
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
        int numberOfBytesWrittenForBackgroundData;
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
        {
            numberOfBytesWrittenForBackgroundData = 1;
        }
        else if (backgroundData.IsSolidColor)
        {
            numberOfBytesWrittenForBackgroundData = 4;
        }
        else
        {
            numberOfBytesWrittenForBackgroundData = 3;
        }

        return (ushort)(NumberOfBytesForMainLevelData + numberOfBytesWrittenForBackgroundData);
    }

    private void WriteLevelStringData(
        RawFileData writer,
        LevelData levelData)
    {
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelTitle));
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelAuthor));
        writer.Write(_stringIdLookup.GetValueOrDefault(levelData.LevelTheme));
    }

    private static void WriteLevelDimensionData(
        RawFileData writer,
        LevelData levelData)
    {
        var levelDimensions = levelData.LevelDimensions;
        writer.Write((ushort)levelDimensions.W);
        writer.Write((ushort)levelDimensions.H);
        writer.Write((ushort)(levelData.LevelStartPositionX ?? LevelReadWriteHelpers.UnspecifiedLevelStartValue));
        writer.Write((ushort)(levelData.LevelStartPositionY ?? LevelReadWriteHelpers.UnspecifiedLevelStartValue));

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
        RawFileData writer,
        LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
        {
            writer.Write((byte)BackgroundType.NoBackgroundSpecified);

            return;
        }

        if (backgroundData.IsSolidColor)
        {
            writer.Write((byte)BackgroundType.SolidColorBackground);
            var actualBackgroundColor = backgroundData.Color;
            writer.Write(actualBackgroundColor.R);
            writer.Write(actualBackgroundColor.G);
            writer.Write(actualBackgroundColor.B);

            return;
        }

        writer.Write((byte)BackgroundType.TextureBackground);
        var backgroundStringId = _stringIdLookup[backgroundData.BackgroundImageName];
        writer.Write(backgroundStringId);
    }
}