using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles;

public static class TerrainArchetypeReadingHelpers
{
    public static TerrainArchetypeData GetTerrainArchetypeData(
        string styleName,
        string pieceName,
        RawFileData rawFileData,
        bool pieceExists)
    {
        if (pieceExists)
            return ReadTerrainArchetypeData(
                styleName,
                pieceName,
                rawFileData);

        return TerrainArchetypeData.CreateTrivialTerrainArchetypeData(
            styleName,
            pieceName);
    }

    private static TerrainArchetypeData ReadTerrainArchetypeData(
        string styleName,
        string pieceName,
        RawFileData rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read8BitUnsignedInteger();
        int initialPosition = rawFileData.Position;

        byte terrainArchetypeDataByte = rawFileData.Read8BitUnsignedInteger();
        LevelReadWriteHelpers.DecipherTerrainArchetypeDataByte(
            terrainArchetypeDataByte,
            out var isSteel,
            out var resizeType);

        int defaultWidth = 0;
        int defaultHeight = 0;

        int nineSliceBottom = 0;
        int nineSliceLeft = 0;
        int nineSliceTop = 0;
        int nineSliceRight = 0;

        if ((resizeType & ResizeType.ResizeHorizontal) != ResizeType.None)
        {
            defaultWidth = rawFileData.Read8BitUnsignedInteger();

            if (defaultWidth > 0)
            {
                nineSliceLeft = rawFileData.Read8BitUnsignedInteger();
                nineSliceRight = rawFileData.Read8BitUnsignedInteger();
            }
        }

        if ((resizeType & ResizeType.ResizeVertical) != ResizeType.None)
        {
            defaultHeight = rawFileData.Read8BitUnsignedInteger();

            if (defaultHeight > 0)
            {
                nineSliceBottom = rawFileData.Read8BitUnsignedInteger();
                nineSliceTop = rawFileData.Read8BitUnsignedInteger();
            }
        }

        AssertTerrainArchetypeBytesMakeSense(
            rawFileData.Position,
            initialPosition,
            numberOfBytesToRead);

        var newTerrainArchetypeData = new TerrainArchetypeData
        {
            Style = styleName,
            TerrainPiece = pieceName,

            IsSteel = isSteel,
            ResizeType = resizeType,

            DefaultWidth = defaultWidth,
            DefaultHeight = defaultHeight,

            NineSliceBottom = nineSliceBottom,
            NineSliceLeft = nineSliceLeft,
            NineSliceTop = nineSliceTop,
            NineSliceRight = nineSliceRight
        };

        return newTerrainArchetypeData;
    }

    private static void AssertTerrainArchetypeBytesMakeSense(
        int currentPosition,
        int initialPosition,
        int expectedByteCount)
    {
        if (currentPosition - initialPosition == expectedByteCount)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for terrain archetype data section! " +
            $"Expected: {expectedByteCount}, Actual: {currentPosition - initialPosition}");
    }
}
