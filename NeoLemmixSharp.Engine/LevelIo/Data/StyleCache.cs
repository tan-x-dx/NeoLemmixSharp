using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.Data.Level;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Terrain;
using NeoLemmixSharp.Engine.LevelIo.Data.Style;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Gadget;
using NeoLemmixSharp.Engine.LevelIo.Data.Style.Terrain;
using NeoLemmixSharp.Engine.LevelIo.Reading;
using NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;
using NeoLemmixSharp.Engine.LevelIo.Versions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelIo.Data;

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
        var notUsedStyles = new List<StyleIdentifier>(8);

        foreach (var kvp in LoadedStyles)
        {
            ref var numberOfLevelsSinceLastUsed = ref kvp.Value.NumberOfLevelsSinceLastUsed;

            numberOfLevelsSinceLastUsed++;

            if (numberOfLevelsSinceLastUsed >= EngineConstants.NumberOfLevelsToKeepStyle)
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
            result.Add(terrainData.StyleIdentifier);
        }

        foreach (var terrainGroupData in levelData.AllTerrainGroups)
        {
            foreach (var terrainData in terrainGroupData.AllBasicTerrainData)
            {
                result.Add(terrainData.StyleIdentifier);
            }
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            result.Add(gadgetData.StyleIdentifier);
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
            var stylePiecePair = terrainData.GetStylePiecePair();
            ref var terrainArchetypeDataForLevel = ref CollectionsMarshal.GetValueRefOrAddDefault(result, stylePiecePair, out var exists);

            if (exists)
                return;

            var terrainStyle = terrainData.StyleIdentifier;

            if (!LoadedStyles.TryGetValue(terrainStyle, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            ref var terrainArchetypeDataForStyle = ref CollectionsMarshal.GetValueRefOrAddDefault(styleData.TerrainArchetypeData, terrainData.TerrainPiece, out exists);
            if (exists)
                return;

            terrainArchetypeDataForStyle = TerrainArchetypeData.CreateTrivialTerrainArchetypeData(stylePiecePair);
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
            ref var gadgetArchetypeData = ref CollectionsMarshal.GetValueRefOrAddDefault(result, gadgetData.GetStylePiecePair(), out var exists);

            if (exists)
                return;

            var gadgetStyle = gadgetData.StyleIdentifier;

            if (!LoadedStyles.TryGetValue(gadgetStyle, out var styleData))
                throw new InvalidOperationException("Style not present in cache!");

            gadgetArchetypeData = styleData.GadgetArchetypeData[gadgetData.GadgetPiece];
        }
    }
}
