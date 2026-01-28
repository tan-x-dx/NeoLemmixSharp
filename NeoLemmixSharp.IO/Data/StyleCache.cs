using NeoLemmixSharp.IO.Data.Level;
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
    public static StyleData DefaultStyleData { get; set; } = null!;

    public static void Initialise()
    {
        if (DefaultStyleData is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(StyleCache)} more than once!");

        DefaultStyleData =
#if DEBUG
            DefaultStyleGenerator.GenerateDefaultStyle();
#else
            FileTypeHandler.ReadStyle(IoConstants.DefaultStyleFormatPair);
#endif

        CachedStyles.Add(IoConstants.DefaultStyleFormatPair, DefaultStyleData);
    }

    public static void EnsureStylesAreLoadedForLevel(LevelData levelData)
    {
        var allMentionedStyleIdentifiers = GetAllMentionedStyles(levelData);

        foreach (var styleIdentifier in allMentionedStyleIdentifiers)
        {
            var styleFormatPair = new StyleFormatPair(styleIdentifier, levelData.FileFormatType);
            GetOrLoadStyleData(styleFormatPair);
        }
    }

    private static HashSet<StyleIdentifier> GetAllMentionedStyles(LevelData levelData)
    {
        var result = new HashSet<StyleIdentifier>(IoConstants.AssumedInitialStyleCapacity)
        {
            levelData.LevelStyle
        };

        foreach (var terrainData in levelData.AllTerrainInstanceData)
        {
            result.Add(terrainData.StyleIdentifier);
        }

        foreach (var terrainGroupData in levelData.AllTerrainGroups)
        {
            foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
            {
                result.Add(terrainData.StyleIdentifier);
            }
        }

        foreach (var gadgetData in levelData.AllGadgetInstanceData)
        {
            result.Add(gadgetData.StyleIdentifier);
        }

        return result;
    }

    public static StyleData GetOrLoadStyleData(StyleFormatPair styleFormatPair)
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

    public static TerrainArchetypeData GetTerrainArchetypeData(StyleIdentifier styleIdentifier, PieceIdentifier pieceIdentifier, FileFormatType fileFormatType)
    {
        var styleFormatPair = new StyleFormatPair(styleIdentifier, fileFormatType);
        var style = GetOrLoadStyleData(styleFormatPair);
        return style.TerrainArchetypeDataLookup[pieceIdentifier];
    }

    public static Dictionary<StylePiecePair, GadgetArchetypeData> GetAllGadgetArchetypeData(LevelData levelData)
    {
        var result = new Dictionary<StylePiecePair, GadgetArchetypeData>(IoConstants.AssumedNumberOfGadgetArchetypeDataInLevel);

        foreach (var prototype in levelData.AllGadgetInstanceData)
        {
            var stylePiecePair = new StylePiecePair(prototype.StyleIdentifier, prototype.PieceIdentifier);
            ref var gadgetArchetypeDataForLevel = ref CollectionsMarshal.GetValueRefOrAddDefault(result, stylePiecePair, out var exists);

            if (exists)
                continue;

            var styleKey = new StyleFormatPair(prototype.StyleIdentifier, levelData.FileFormatType);

            if (!CachedStyles.TryGetValue(styleKey, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            gadgetArchetypeDataForLevel = styleData.GadgetArchetypeDataLookup[prototype.PieceIdentifier];
        }

        return result;
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
