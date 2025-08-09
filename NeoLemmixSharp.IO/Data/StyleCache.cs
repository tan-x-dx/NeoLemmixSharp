using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data;

public static class StyleCache
{
    private static readonly Dictionary<StyleFormatPair, StyleData> CachedStyles = new(IoConstants.AssumedInitialStyleCapacity * IoConstants.NumberOfLevelsToKeepStyle);
    internal static StyleData DefaultStyleData { get; set; } = null!;

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

        CachedStyles.Add(IoConstants.DefaultStyleFormatPair, DefaultStyleData);
    }

    public static void EnsureStylesAreLoadedForLevel(LevelData levelData)
    {
        var allMentionedStyleFormatPairs = GetAllMentionedStyles(levelData);

        foreach (var styleFormatPair in allMentionedStyleFormatPairs)
        {
            GetOrLoadStyleData(styleFormatPair);
        }
    }

    internal static StyleData GetOrLoadStyleData(StyleFormatPair styleFormatPair)
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

        return styleData;
    }

    private static HashSet<StyleFormatPair> GetAllMentionedStyles(LevelData levelData)
    {
        var result = new HashSet<StyleFormatPair>(IoConstants.AssumedInitialStyleCapacity)
        {
            new(levelData.LevelTheme, levelData.FileFormatType)
        };

        foreach (var terrainData in levelData.AllTerrainData)
        {
            result.Add(new StyleFormatPair(terrainData.StyleIdentifier, levelData.FileFormatType));
        }

        foreach (var terrainGroupData in levelData.AllTerrainGroups)
        {
            foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
            {
                result.Add(new StyleFormatPair(terrainData.StyleIdentifier, levelData.FileFormatType));
            }
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            result.Add(new StyleFormatPair(gadgetData.StyleIdentifier, levelData.FileFormatType));
        }

        return result;
    }

    public static Dictionary<StylePiecePair, TerrainArchetypeData> GetAllTerrainArchetypeData(LevelData levelData)
    {
        var result = new Dictionary<StylePiecePair, TerrainArchetypeData>(IoConstants.AssumedNumberOfTerrainArchetypeDataInLevel);

        foreach (var prototype in levelData.AllTerrainData)
        {
            FetchTerrainArchetypeData(prototype);
        }

        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            foreach (var prototype in terrainGroup.AllBasicTerrainData)
            {
                FetchTerrainArchetypeData(prototype);
            }
        }

        return result;

        void FetchTerrainArchetypeData(TerrainData terrainData)
        {
            ref var terrainArchetypeDataForLevel = ref CollectionsMarshal.GetValueRefOrAddDefault(result, terrainData.GetStylePiecePair(), out var exists);

            if (exists)
                return;

            var key = new StyleFormatPair(terrainData.StyleIdentifier, levelData.FileFormatType);

            if (!CachedStyles.TryGetValue(key, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            ref var terrainArchetypeDataForStyle = ref CollectionsMarshal.GetValueRefOrAddDefault(styleData.TerrainArchetypeDataLookup, terrainData.PieceIdentifier, out exists);
            if (!exists)
            {
                terrainArchetypeDataForStyle = TerrainArchetypeData.CreateTrivialTerrainArchetypeData(terrainData.StyleIdentifier, terrainData.PieceIdentifier);
            }
            terrainArchetypeDataForLevel = terrainArchetypeDataForStyle;
        }
    }

    public static Dictionary<StylePiecePair, IGadgetArchetypeData> GetAllGadgetArchetypeData(LevelData levelData)
    {
        var result = new Dictionary<StylePiecePair, IGadgetArchetypeData>(IoConstants.AssumedNumberOfGadgetArchetypeDataInLevel);

        foreach (var prototype in levelData.AllGadgetData)
        {
            FetchGadgetArchetypeData(prototype);
        }

        return result;

        void FetchGadgetArchetypeData(GadgetData gadgetData)
        {
            ref var gadgetArchetypeDataForLevel = ref CollectionsMarshal.GetValueRefOrAddDefault(result, gadgetData.GetStylePiecePair(), out var exists);

            if (exists)
                return;

            var key = new StyleFormatPair(gadgetData.StyleIdentifier, levelData.FileFormatType);

            if (!CachedStyles.TryGetValue(key, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            gadgetArchetypeDataForLevel = styleData.GadgetArchetypeDataLookup[gadgetData.PieceIdentifier];
        }
    }

    public static ThemeData GetThemeData(StyleFormatPair styleFormatPair)
    {
        return CachedStyles[styleFormatPair].ThemeData;
    }

    public static void CleanUpOldStyles()
    {
        var notUsedStylesFormatPairs = new List<StyleFormatPair>(IoConstants.AssumedInitialStyleCapacity);

        foreach (var kvp in CachedStyles)
        {
            ref var numberOfLevelsSinceLastUsed = ref kvp.Value.NumberOfLevelsSinceLastUsed;

            numberOfLevelsSinceLastUsed++;

            if (numberOfLevelsSinceLastUsed > IoConstants.NumberOfLevelsToKeepStyle)
            {
                notUsedStylesFormatPairs.Add(kvp.Key);
            }
        }

        foreach (var styleFormatPair in notUsedStylesFormatPairs)
        {
            if (IoConstants.DefaultStyleFormatPair.Equals(styleFormatPair))
                continue;

            CachedStyles.Remove(styleFormatPair);
        }
    }
}
