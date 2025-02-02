using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public static class TerrainArchetypeReadingHelpers
{
    public static void ReadTerrainArchetypeData(
        LevelData levelData,
        RawFileData rawFileData)
    {

    }

    public static void CreateTrivialTerrainArchetypeData(
        LevelData levelData,
        string styleName,
        string pieceName)
    {
        var newTerrainArchetypeData = new TerrainArchetypeData
        {
            Style = styleName,
            TerrainPiece = pieceName,

            IsSteel = false,
            ResizeType = ResizeType.None,

            NineSliceRight = 0,
            NineSliceTop = 0,
            NineSliceLeft = 0,
            NineSliceBottom = 0,

            DefaultWidth = 0,
            DefaultHeight = 0,
        };

        levelData.TerrainArchetypeData.Add(
            new LevelData.StylePiecePair(styleName, pieceName),
            newTerrainArchetypeData);
    }
}
