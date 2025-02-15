using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class CommonHelpers
{
    public static HashSetLookup<string, string> GetUniqueStyles(
        LevelData levelData,
        StylePieceType dataType)
    {
        var uniqueStyles = new HashSetLookup<string, string>();

        if (dataType == StylePieceType.Terrain)
        {
            foreach (var terrainData in levelData.AllTerrainData)
            {
                uniqueStyles.Add(terrainData.Style, terrainData.TerrainPiece);
            }
        }
        else
        {
            foreach (var gadgetData in levelData.AllGadgetData)
            {
                uniqueStyles.Add(gadgetData.Style, gadgetData.GadgetPiece);
            }
        }

        if (uniqueStyles.KeyCount == 0)
            throw new LevelReadingException("No styles specified");

        return uniqueStyles;
    }
}

public enum StylePieceType : byte
{
    Terrain = 0x00,
    Gadget = 0x01
}

