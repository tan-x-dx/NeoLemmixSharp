using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public readonly ref struct LevelDataComponentWriter
{
    private const byte NoBackgroundSpecified = 0x00;
    private const byte SolidBackgroundColor = 0x01;
    private const byte BackgroundImageSpecified = 0x02;

    private const int UnspecifiedLevelStartValue = 5000;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelDataComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    private static ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x79, 0xA6];
        return sectionIdentifier;
    }

    private static ushort CalculateNumberOfItemsInSection()
    {
        return 1;
    }

    public void WriteSection(BinaryWriter writer, LevelData levelData)
    {
        writer.Write(GetSectionIdentifier());
        writer.Write(CalculateNumberOfItemsInSection());

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

        return (ushort)(6 + 9 + 16 + numberOfBytesWrittenForBackgroundData);
    }

    private void WriteLevelStringData(BinaryWriter writer, LevelData levelData)
    {
        var titleStringId = _stringIdLookup.GetValueOrDefault(levelData.LevelTitle);
        var authorStringId = _stringIdLookup.GetValueOrDefault(levelData.LevelAuthor);
        var levelThemeStringId = _stringIdLookup.GetValueOrDefault(levelData.LevelTheme);

        writer.Write(titleStringId);
        writer.Write(authorStringId);
        writer.Write(levelThemeStringId);
    }

    private static void WriteLevelDimensionData(BinaryWriter writer, LevelData levelData)
    {
        writer.Write((ushort)levelData.LevelWidth);
        writer.Write((ushort)levelData.LevelHeight);
        writer.Write((ushort)(levelData.LevelStartPositionX ?? UnspecifiedLevelStartValue));
        writer.Write((ushort)(levelData.LevelStartPositionY ?? UnspecifiedLevelStartValue));

        var boundaryByte = GetBoundaryBehaviourByte(levelData);
        writer.Write(boundaryByte);
    }

    private static byte GetBoundaryBehaviourByte(LevelData levelData)
    {
        var horizontalData = (int)levelData.HorizontalBoundaryBehaviour;
        var verticalData = (int)levelData.VerticalBoundaryBehaviour;
        var combined = (verticalData << 1) | horizontalData;

        return (byte)combined;
    }

    private void WriteLevelBackgroundData(BinaryWriter writer, LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
        {
            writer.Write(NoBackgroundSpecified);

            return;
        }

        if (backgroundData.IsSolidColor)
        {
            writer.Write(SolidBackgroundColor);
            var actualBackgroundColor = backgroundData.Color;
            writer.Write(actualBackgroundColor.R);
            writer.Write(actualBackgroundColor.G);
            writer.Write(actualBackgroundColor.B);

            return;
        }

        writer.Write(BackgroundImageSpecified);
        var backgroundStringId = _stringIdLookup[backgroundData.BackgroundImageName];
        writer.Write(backgroundStringId);
    }
}