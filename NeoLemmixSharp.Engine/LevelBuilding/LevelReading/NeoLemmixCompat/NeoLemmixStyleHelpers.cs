using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public static class NeoLemmixStyleHelpers
{
    private readonly struct PieceAndFileLink(string pieceName, string? correspondingDataFile)
    {
        public readonly string PieceName = pieceName;
        public readonly string? CorrespondingDataFile = correspondingDataFile;
    }

    public static void ProcessTerrainArchetypeData(
        LevelData levelData)
    {
        var uniqueTerrainDataStyles = GetUniqueStyles(levelData, StylePieceType.Terrain);
        levelData.TerrainArchetypeData.EnsureCapacity(uniqueTerrainDataStyles.ValueCount);

        // Reuse this array to help minimize allocations
        var singletonArray = new NeoLemmixDataReader[1];

        foreach (var styleGroup in uniqueTerrainDataStyles)
        {
            ProcessTerrainStyleData(levelData, styleGroup, singletonArray);
        }
    }

    private static void ProcessTerrainStyleData(
        LevelData levelData,
        HashSetLookup<string, string>.Group styleGroup,
        NeoLemmixDataReader[] singletonArray)
    {
        var styleFolderPath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            styleGroup.Key,
            DefaultFileExtensions.TerrainFolderName);

        var files = Directory.GetFiles(styleFolderPath);
        var neoLemmixDataFileLinks = FindNeoLemmixDataFiles(styleGroup, files, NeoLemmixFileExtensions.TerrainFileExtension);

        ProcessTerrainArchetypeData(levelData, styleGroup.Key, neoLemmixDataFileLinks, singletonArray);
    }

    private static void ProcessTerrainArchetypeData(
        LevelData levelData,
        string styleName,
        PieceAndFileLink[] neoLemmixDataFileLinks,
        NeoLemmixDataReader[] singletonArray)
    {
        foreach (var pieceAndFileLink in neoLemmixDataFileLinks)
        {
            TerrainArchetypeData newTerrainArchetypeData;
            if (pieceAndFileLink.CorrespondingDataFile is null)
            {
                newTerrainArchetypeData = TerrainArchetypeData.CreateTrivialTerrainArchetypeData(
                    styleName,
                    pieceAndFileLink.PieceName);
            }
            else
            {
                var terrainArchetypeDataReader = new TerrainArchetypeDataReader(styleName, pieceAndFileLink.PieceName);
                singletonArray[0] = terrainArchetypeDataReader;

                new DataReaderList(
                    pieceAndFileLink.CorrespondingDataFile,
                    singletonArray)
                    .ReadFile();
                newTerrainArchetypeData = terrainArchetypeDataReader.CreateTerrainArchetypeData();
            }

            levelData.TerrainArchetypeData.Add(
                new LevelData.StylePiecePair(styleName, pieceAndFileLink.PieceName),
                newTerrainArchetypeData);
        }
    }

    public static void ProcessGadgetArchetypeData(
        LevelData levelData,
        UniqueStringSet uniqueStringSet)
    {
        var uniqueGadgetDataStyles = GetUniqueStyles(levelData, StylePieceType.Gadget);
        levelData.AllGadgetArchetypeBuilders.EnsureCapacity(uniqueGadgetDataStyles.ValueCount);

        // Reuse this array to help minimize allocations
        var dataReaders = new NeoLemmixDataReader[3];

        foreach (var styleGroup in uniqueGadgetDataStyles)
        {
            ProcessGadgetStyleData(levelData, uniqueStringSet, styleGroup, dataReaders);
        }
    }

    private static void ProcessGadgetStyleData(
        LevelData levelData,
        UniqueStringSet uniqueStringSet,
        HashSetLookup<string, string>.Group styleGroup,
        NeoLemmixDataReader[] dataReaders)
    {
        var styleFolderPath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            styleGroup.Key,
            DefaultFileExtensions.GadgetFolderName);

        var files = Directory.GetFiles(styleFolderPath);
        var neoLemmixDataFileLinks = FindNeoLemmixDataFiles(styleGroup, files, NeoLemmixFileExtensions.GadgetFileExtension);

        ProcessGadgetArchetypeData(levelData, uniqueStringSet, styleGroup.Key, neoLemmixDataFileLinks, dataReaders);
    }

    private static void ProcessGadgetArchetypeData(
        LevelData levelData,
        UniqueStringSet uniqueStringSet,
        string styleName,
        PieceAndFileLink[] neoLemmixDataFileLinks,
        NeoLemmixDataReader[] dataReaders)
    {
        foreach (var pieceAndFileLink in neoLemmixDataFileLinks)
        {
            if (pieceAndFileLink.CorrespondingDataFile is null)
                throw new InvalidOperationException($"Could not locate NeoLemmix gadget data for gadget: {pieceAndFileLink.PieceName}");

            var gadgetArchetypeData = new NeoLemmixGadgetArchetypeData()
            {
                Style = styleName,
                GadgetPiece = pieceAndFileLink.PieceName
            };
            dataReaders[0] = new GadgetArchetypeDataReader(gadgetArchetypeData);
            dataReaders[1] = new PrimaryAnimationReader(gadgetArchetypeData);
            dataReaders[2] = new SecondaryAnimationReader(uniqueStringSet, gadgetArchetypeData);

            new DataReaderList(
                pieceAndFileLink.CorrespondingDataFile,
                dataReaders)
                .ReadFile();
        }
    }

    private static PieceAndFileLink[] FindNeoLemmixDataFiles(
        HashSetLookup<string, string>.Group styleGroup,
        ReadOnlySpan<string> files,
        ReadOnlySpan<char> fileExtensionToLocate)
    {
        var result = new PieceAndFileLink[styleGroup.Count];
        var i = 0;

        foreach (var piece in styleGroup)
        {
            var correspondingDataFile = TryGetCorrespondingDataFile(files, piece, fileExtensionToLocate);
            result[i++] = new PieceAndFileLink(piece, correspondingDataFile);
        }

        return result;

        static string? TryGetCorrespondingDataFile(
            ReadOnlySpan<string> files,
            ReadOnlySpan<char> piece,
            ReadOnlySpan<char> fileExtensionToLocate)
        {
            foreach (var fileName in files)
            {
                var fileNameSpan = fileName.AsSpan();

                if (Path.GetExtension(fileNameSpan).Equals(fileExtensionToLocate, StringComparison.OrdinalIgnoreCase) &&
                    Path.GetFileNameWithoutExtension(fileNameSpan).Equals(piece, StringComparison.OrdinalIgnoreCase))
                    return fileName;
            }

            return null;
        }
    }

    private static HashSetLookup<string, string> GetUniqueStyles(
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
