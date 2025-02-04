﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public static class StyleHelpers
{
    private enum DataType
    {
        TerrainData,
        GadgetData
    }

    private readonly struct PieceAndFileLink(string pieceName, string? correspondingDataFile)
    {
        public readonly string PieceName = pieceName;
        public readonly string? CorrespondingDataFile = correspondingDataFile;
    }

    public static void ProcessStyleArchetypeData(
        LevelData levelData,
        UniqueStringSet uniqueStringSet)
    {
        var uniqueTerrainDataStyles = GetUniqueStyles(levelData, DataType.TerrainData);
        levelData.TerrainArchetypeData.EnsureCapacity(uniqueTerrainDataStyles.ValueCount);

        foreach (var styleGroup in uniqueTerrainDataStyles)
        {
            ProcessStyleData(levelData, uniqueStringSet, styleGroup, DataType.TerrainData);
        }

        var uniqueGadgetDataStyles = GetUniqueStyles(levelData, DataType.GadgetData);
        levelData.AllGadgetArchetypeBuilders.EnsureCapacity(uniqueGadgetDataStyles.ValueCount);

        foreach (var styleGroup in uniqueGadgetDataStyles)
        {
            ProcessStyleData(levelData, uniqueStringSet, styleGroup, DataType.GadgetData);
        }
    }

    private static HashSetLookup<string, string> GetUniqueStyles(
        LevelData levelData,
        DataType dataType)
    {
        var uniqueStyles = new HashSetLookup<string, string>();

        if (dataType == DataType.TerrainData)
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

    private static void ProcessStyleData(
        LevelData levelData,
        UniqueStringSet uniqueStringSet,
        HashSetLookup<string, string>.Group styleGroup,
        DataType dataType)
    {
        string subFolderName;
        string fileExtension;

        if (dataType == DataType.TerrainData)
        {
            subFolderName = DefaultFileExtensions.TerrainFolderName;
            fileExtension = NeoLemmixFileExtensions.TerrainFileExtension;
        }
        else
        {
            subFolderName = DefaultFileExtensions.GadgetFolderName;
            fileExtension = NeoLemmixFileExtensions.GadgetFileExtension;
        }

        var styleFolderPath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            styleGroup.Key,
            subFolderName);

        var files = Directory.GetFiles(styleFolderPath);
        var neoLemmixDataFileLinks = FindNeoLemmixDataFiles(styleGroup, files, fileExtension);

        if (dataType == DataType.TerrainData)
        {
            ProcessTerrainArchetypeData(levelData, styleGroup.Key, neoLemmixDataFileLinks);
        }
        else
        {
            ProcessGadgetArchetypeData(levelData, uniqueStringSet, styleGroup.Key, neoLemmixDataFileLinks);
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

    private static void ProcessTerrainArchetypeData(
        LevelData levelData,
        string styleName,
        PieceAndFileLink[] neoLemmixDataFileLinks)
    {
        // Reuse this array to help minimize allocations
        var singletonArray = new NeoLemmixDataReader[1];

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

    private static void ProcessGadgetArchetypeData(
        LevelData levelData,
        UniqueStringSet uniqueStringSet,
        string styleName,
        PieceAndFileLink[] neoLemmixDataFileLinks)
    {
        // Reuse this array to help minimize allocations
        var dataReaders = new NeoLemmixDataReader[3];

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
}
