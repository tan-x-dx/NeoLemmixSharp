using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class LevelDataComponentReader : ILevelDataReader
{
    private readonly Dictionary<ushort, string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelDataSectionIdentifier;

    public LevelDataComponentReader(Dictionary<ushort, string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItemsInSection = reader.Read16BitUnsignedInteger();
        LevelReadWriteHelpers.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level data item!");

        var numberOfBytesToRead = reader.Read16BitUnsignedInteger();
        var initialBytesRead = reader.BytesRead;

        var stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelTitle = _stringIdLookup[stringId];

        stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelAuthor = _stringIdLookup[stringId];

        stringId = reader.Read16BitUnsignedInteger();
        levelData.LevelTheme = _stringIdLookup[stringId];

        levelData.LevelId = reader.Read64BitUnsignedInteger();
        levelData.Version = reader.Read64BitUnsignedInteger();

        ReadLevelDimensionData(reader, levelData);
        ReadBackgroundData(reader, levelData);

        AssertLevelDataBytesMakeSense(
            reader.BytesRead,
            initialBytesRead,
            numberOfBytesToRead);
    }

    private static void ReadLevelDimensionData(BinaryReaderWrapper reader, LevelData levelData)
    {
        levelData.LevelWidth = reader.Read16BitUnsignedInteger();
        levelData.LevelHeight = reader.Read16BitUnsignedInteger();

        var value = reader.Read16BitUnsignedInteger();
        if (value != LevelReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionX = value;
        }

        value = reader.Read16BitUnsignedInteger();
        if (value != LevelReadWriteHelpers.UnspecifiedLevelStartValue)
        {
            levelData.LevelStartPositionY = value;
        }

        var boundaryByte = reader.Read8BitUnsignedInteger();

        DecipherBoundaryBehaviours(levelData, boundaryByte);
    }

    private static void DecipherBoundaryBehaviours(LevelData levelData, byte boundaryByte)
    {
        var horizontalBoundaryBehaviour = (BoundaryBehaviourType)(boundaryByte & 1);
        var verticalBoundaryBehaviour = (BoundaryBehaviourType)((boundaryByte >> 1) & 1);

        levelData.HorizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        levelData.VerticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    private void ReadBackgroundData(BinaryReaderWrapper reader, LevelData levelData)
    {
        var specifierByte = reader.Read8BitUnsignedInteger();

        levelData.LevelBackground = specifierByte switch
        {
            LevelReadWriteHelpers.NoBackgroundSpecified => null,
            LevelReadWriteHelpers.SolidColorBackground => ReadSolidColorBackgroundData(),
            LevelReadWriteHelpers.TextureBackground => ReadTextureBackgroundData(),

            _ => throw new LevelReadingException($"Unknown background specifier byte: {specifierByte:X}")
        };

        return;

        [SkipLocalsInit]
        BackgroundData ReadSolidColorBackgroundData()
        {
            Span<byte> buffer = stackalloc byte[3];
            reader.ReadBytes(buffer);

            return new BackgroundData
            {
                IsSolidColor = true,
                Color = new Color(r: buffer[0], g: buffer[1], b: buffer[2], alpha: (byte)0xff),
                BackgroundImageName = string.Empty
            };
        }

        BackgroundData ReadTextureBackgroundData()
        {
            var backgroundStringId = reader.Read16BitUnsignedInteger();

            return new BackgroundData
            {
                IsSolidColor = false,
                Color = Color.Black,
                BackgroundImageName = _stringIdLookup[backgroundStringId]
            };
        }
    }

    private static void AssertLevelDataBytesMakeSense(
        int bytesRead,
        int initialBytesRead,
        int numberOfBytesToRead)
    {
        if (bytesRead - initialBytesRead == numberOfBytesToRead)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for level data section! " +
            $"Expected: {numberOfBytesToRead}, Actual: {bytesRead - initialBytesRead}");
    }
}