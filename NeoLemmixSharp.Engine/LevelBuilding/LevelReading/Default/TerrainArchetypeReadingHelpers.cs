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
        var newTerrainArchetypeData = TerrainArchetypeData.CreateTrivialTerrainArchetypeData(
            styleName,
            pieceName);

        levelData.TerrainArchetypeData.Add(
            new LevelData.StylePiecePair(styleName, pieceName),
            newTerrainArchetypeData);
    }
}
