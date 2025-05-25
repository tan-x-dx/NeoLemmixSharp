using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data.Style;

public static class StyleCache
{
    internal static readonly StyleIdentifier DefaultStyleIdentifier = new(EngineConstants.DefaultStyleIdentifier);

    internal static readonly StyleFormatPair DefaultStyleFormatPair = new(
        DefaultStyleIdentifier,
        FileFormatType.Default);

    private static readonly Dictionary<StyleFormatPair, StyleData> CachedStyles = new(EngineConstants.AssumedInitialStyleCapacity * EngineConstants.NumberOfLevelsToKeepStyle);
    private static StyleData DefaultStyleData { get; set; } = null!;

    public static void Initialise()
    {
        if (DefaultStyleData is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(StyleCache)} more than once!");

        DefaultStyleData =
#if DEBUG
            DefaultStyleGenerator.GenerateDefaultStyle();
#else
            FileTypeHandler.ReadStyle(DefaultStyleFormatPair);
#endif

        CachedStyles.Add(DefaultStyleFormatPair, DefaultStyleData);
    }

    public static void EnsureStylesAreLoadedForLevel(LevelData levelData)
    {
        var allMentionedStyleFormatPairs = GetAllMentionedStyles(levelData);

        foreach (var styleFormatPair in allMentionedStyleFormatPairs)
        {
            ref var styleData = ref CollectionsMarshal.GetValueRefOrAddDefault(CachedStyles, styleFormatPair, out var exists);

            if (exists)
            {
                styleData!.NumberOfLevelsSinceLastUsed = 0;
            }
            else
            {
                styleData = FileTypeHandler.ReadStyle(styleFormatPair);
            }
        }
    }

    private static HashSet<StyleFormatPair> GetAllMentionedStyles(LevelData levelData)
    {
        var result = new HashSet<StyleFormatPair>(EngineConstants.AssumedInitialStyleCapacity)
        {
            new(levelData.LevelTheme, levelData.FileFormatType)
        };

        foreach (var terrainData in levelData.AllTerrainData)
        {
            result.Add(new StyleFormatPair(terrainData.StyleName, levelData.FileFormatType));
        }

        foreach (var terrainGroupData in levelData.AllTerrainGroups)
        {
            foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
            {
                result.Add(new StyleFormatPair(terrainData.StyleName, levelData.FileFormatType));
            }
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            result.Add(new StyleFormatPair(gadgetData.StyleName, levelData.FileFormatType));
        }

        return result;
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

            var key = new StyleFormatPair(terrainData.StyleName, levelData.FileFormatType);

            if (!CachedStyles.TryGetValue(key, out var styleData))
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

            var key = new StyleFormatPair(gadgetData.StyleName, levelData.FileFormatType);

            if (!CachedStyles.TryGetValue(key, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            gadgetArchetypeDataForLevel = styleData.GadgetArchetypeData[gadgetData.PieceName];
        }
    }

    public static ThemeData GetThemeData(StyleFormatPair styleFormatPair)
    {
        return CachedStyles[styleFormatPair].ThemeData;
    }

    public static void CleanUpOldStyles()
    {
        var notUsedStylesFormatPairs = new List<StyleFormatPair>(EngineConstants.AssumedInitialStyleCapacity);

        foreach (var kvp in CachedStyles)
        {
            ref var numberOfLevelsSinceLastUsed = ref kvp.Value.NumberOfLevelsSinceLastUsed;

            numberOfLevelsSinceLastUsed++;

            if (numberOfLevelsSinceLastUsed > EngineConstants.NumberOfLevelsToKeepStyle)
            {
                notUsedStylesFormatPairs.Add(kvp.Key);
            }
        }

        foreach (var styleFormatPair in notUsedStylesFormatPairs)
        {
            if (DefaultStyleFormatPair.Equals(styleFormatPair))
                continue;

            CachedStyles.Remove(styleFormatPair);
        }
    }
}
