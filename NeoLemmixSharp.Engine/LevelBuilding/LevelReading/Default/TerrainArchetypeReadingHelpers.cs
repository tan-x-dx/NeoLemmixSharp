using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public static class TerrainArchetypeReadingHelpers
{
    public static void ReadTerrainArchetypeData(
        LevelData levelData,
        string styleName,
        string pieceName,
        RawFileData rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read8BitUnsignedInteger();
        int initialBytesRead = rawFileData.BytesRead;

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
            rawFileData.BytesRead,
            initialBytesRead,
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

        levelData.TerrainArchetypeData.Add(
            new LevelData.StylePiecePair(styleName, pieceName),
            newTerrainArchetypeData);
    }

    private static void AssertTerrainArchetypeBytesMakeSense(
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

    public static void CreateTrivialTerrainArchetypeData(
        LevelData levelData,
        string styleName,
        string pieceName)
    {
        var newTerrainArchetypeData = TerrainArchetypeData.CreateTrivialTerrainArchetypeData(
            styleName,
            pieceName);

        levelData.TerrainArchetypeData.Add(
            new LevelData.StylePiecePair(styleName, pieceName),
            newTerrainArchetypeData);
    }
}
