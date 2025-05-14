using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Reading.Styles;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data;

public static class StyleCache
{
    private static readonly Dictionary<StyleIdentifier, StyleData> CachedStyles = new(EngineConstants.AssumedInitialStyleCapacity * EngineConstants.NumberOfLevelsToKeepStyle);

    public static void EnsureStylesAreLoadedForLevel(LevelData levelData)
    {
        var allStyles = GetAllMentionedStyles(levelData);

        foreach (var style in allStyles)
        {
            ref var styleData = ref CollectionsMarshal.GetValueRefOrAddDefault(CachedStyles, style, out var exists);

            if (exists)
            {
                styleData!.NumberOfLevelsSinceLastUsed = 0;
            }
            else
            {
                styleData = LoadStyle(style);
            }
        }
    }

    private static HashSet<StyleIdentifier> GetAllMentionedStyles(LevelData levelData)
    {
        var result = new HashSet<StyleIdentifier>(EngineConstants.AssumedInitialStyleCapacity);

        foreach (var terrainData in levelData.AllTerrainData)
        {
            result.Add(terrainData.StyleName);
        }

        foreach (var terrainGroupData in levelData.AllTerrainGroups)
        {
            foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
            {
                result.Add(terrainData.StyleName);
            }
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            result.Add(gadgetData.StyleName);
        }

        return result;
    }

    private static StyleData LoadStyle(StyleIdentifier style)
    {
        return new StyleReader(style).LoadStyle();
    }

    public static Dictionary<StylePiecePair, TerrainArchetypeData> GetAllTerrainArchetypeData(LevelData levelData)
    {
        var result = new Dictionary<StylePiecePair, TerrainArchetypeData>(EngineConstants.AssumedNumberOfTerrainArchetypeDataInLevel);

        foreach (var terrainData in levelData.AllTerrainData)
        {
            FetchTerrainArchetypeData(terrainData);
        }

        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            foreach (var terrainData in terrainGroup.AllBasicTerrainData)
            {
                FetchTerrainArchetypeData(terrainData);
            }
        }

        return result;

        void FetchTerrainArchetypeData(TerrainData terrainData)
        {
            ref var terrainArchetypeDataForLevel = ref CollectionsMarshal.GetValueRefOrAddDefault(result, terrainData.GetStylePiecePair(), out var exists);

            if (exists)
                return;

            var terrainStyle = terrainData.StyleName;

            if (!CachedStyles.TryGetValue(terrainStyle, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            ref var terrainArchetypeDataForStyle = ref CollectionsMarshal.GetValueRefOrAddDefault(styleData.TerrainArchetypeData, terrainData.PieceName, out exists);
            if (!exists)
            {
                terrainArchetypeDataForStyle = TerrainArchetypeData.CreateTrivialTerrainArchetypeData(terrainData.StyleName, terrainData.PieceName);
            }
            terrainArchetypeDataForLevel = terrainArchetypeDataForStyle;
        }
    }

    public static Dictionary<StylePiecePair, GadgetArchetypeData> GetAllGadgetArchetypeData(LevelData levelData)
    {
        var result = new Dictionary<StylePiecePair, GadgetArchetypeData>(EngineConstants.AssumedNumberOfGadgetArchetypeDataInLevel);

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            FetchGadgetArchetypeData(gadgetData);
        }

        return result;

        void FetchGadgetArchetypeData(GadgetData gadgetData)
        {
            ref var gadgetArchetypeDataForLevel = ref CollectionsMarshal.GetValueRefOrAddDefault(result, gadgetData.GetStylePiecePair(), out var exists);

            if (exists)
                return;

            var gadgetStyle = gadgetData.StyleName;

            if (!CachedStyles.TryGetValue(gadgetStyle, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            gadgetArchetypeDataForLevel = styleData.GadgetArchetypeData[gadgetData.PieceName];
        }
    }

    public static void CleanUpOldStyles()
    {
        var notUsedStyles = new List<StyleIdentifier>(EngineConstants.AssumedInitialStyleCapacity);

        foreach (var kvp in CachedStyles)
        {
            ref var numberOfLevelsSinceLastUsed = ref kvp.Value.NumberOfLevelsSinceLastUsed;

            numberOfLevelsSinceLastUsed++;

            if (numberOfLevelsSinceLastUsed > EngineConstants.NumberOfLevelsToKeepStyle)
            {
                notUsedStyles.Add(kvp.Key);
            }
        }

        foreach (var style in notUsedStyles)
        {
            CachedStyles.Remove(style);
        }
    }
}
