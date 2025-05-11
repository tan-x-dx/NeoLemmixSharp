using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Reading;
using NeoLemmixSharp.IO.Reading.Styles.Sections;
using NeoLemmixSharp.IO.Versions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data;

public static class StyleCache
{
    private static readonly Dictionary<StyleIdentifier, StyleData> LoadedStyles = new(EngineConstants.AssumedInitialStyleCapacity * EngineConstants.NumberOfLevelsToKeepStyle);

    public static void EnsureStylesAreLoadedForLevel(LevelData levelData)
    {
        var allStyles = GetAllMentionedStyles(levelData);

        foreach (var style in allStyles)
        {
            ref var styleData = ref CollectionsMarshal.GetValueRefOrAddDefault(LoadedStyles, style, out var exists);

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

    public static void CleanUpOldStyles()
    {
        var notUsedStyles = new List<StyleIdentifier>(EngineConstants.AssumedInitialStyleCapacity);

        foreach (var kvp in LoadedStyles)
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
            LoadedStyles.Remove(style);
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
        var styleFolderPath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            style.ToString());

        var files = Directory.GetFiles(styleFolderPath);
        if (!TryLocateStyleFile(files, out var styleFilePath))
            throw new FileReadingException($"Could not locate style file in folder: {styleFolderPath}");

        using var fileStream = new FileStream(styleFilePath, FileMode.Open);
        var rawFileData = new RawStyleFileDataReader(fileStream);

        var sectionReaders = VersionHelper.GetStyleDataSectionReadersForVersion(rawFileData.Version);
        var result = new StyleData(style);

        foreach (var sectionReader in sectionReaders)
        {
            ReadSection(rawFileData, result, sectionReader);
        }

        return result;
    }

    private static void ReadSection(
        RawStyleFileDataReader rawFileData,
        StyleData result, StyleDataSectionReader sectionReader)
    {
        var sectionIdentifier = sectionReader.SectionIdentifier;

        if (!rawFileData.TryGetSectionInterval(sectionIdentifier, out var interval))
        {
            FileReadingException.ReaderAssert(
                !sectionReader.IsNecessary,
                "No data for necessary section!");
            return;
        }

        rawFileData.SetReaderPosition(interval.Start);

        var sectionIdentifierBytes = rawFileData.ReadBytes(LevelFileSectionIdentifierHasher.NumberOfBytesForLevelSectionIdentifier);

        FileReadingException.ReaderAssert(
            sectionIdentifierBytes.SequenceEqual(sectionReader.GetSectionIdentifierBytes()),
            "Section Identifier mismatch!");

        sectionReader.ReadSection(rawFileData, result);

        FileReadingException.ReaderAssert(
            interval.Start + interval.Length == rawFileData.Position,
            "Byte reading mismatch!");
    }

    private static bool TryLocateStyleFile(
        ReadOnlySpan<string> allFiles,
        [MaybeNullWhen(false)] out string foundFilePath)
    {
        foreach (var file in allFiles)
        {
            var fileExtension = Path.GetExtension(file.AsSpan());

            if (fileExtension.Equals(DefaultFileExtensions.LevelStyleExtension, StringComparison.OrdinalIgnoreCase))
            {
                foundFilePath = file;
                return true;
            }
        }

        foundFilePath = null;
        return false;
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

            if (!LoadedStyles.TryGetValue(terrainStyle, out var styleData))
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

            if (!LoadedStyles.TryGetValue(gadgetStyle, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            gadgetArchetypeDataForLevel = styleData.GadgetArchetypeData[gadgetData.PieceName];
        }
    }
}
