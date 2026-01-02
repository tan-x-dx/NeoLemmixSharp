using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;
using NeoLemmixSharp.IO.Util;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat;

internal readonly ref struct NeoLemmixStyleReader : IStyleReader<NeoLemmixStyleReader>
{
    private readonly StyleData _styleData;
    private readonly UniqueStringSet _uniqueStringSet = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NeoLemmixStyleReader Create(StyleIdentifier styleIdentifier) => new(styleIdentifier);

    private NeoLemmixStyleReader(StyleIdentifier styleIdentifier)
    {
        _styleData = new StyleData(styleIdentifier, FileFormats.FileFormatType.NeoLemmix);
    }

    public StyleData ReadStyle()
    {
        var styleFolderPath = _styleData.Identifier.GetFolderFilePath();

        _styleData.ThemeData = ReadThemeData(styleFolderPath);

        ReadTerrainArchetypeData(styleFolderPath);
        ReadGadgetArchetypeData(styleFolderPath);

        return _styleData;
    }

    private ThemeData ReadThemeData(string styleFolderPath)
    {
        var themeDataFilePaths = Helpers.GetFilePathsWithExtension(styleFolderPath, NeoLemmixFileExtensions.ThemeFileExtension);

        if (themeDataFilePaths.Length == 1)
            return ReadThemeDataFromFilePath(themeDataFilePaths[0]);

        return StyleCache.DefaultStyleData.ThemeData;
    }

    private ThemeData ReadThemeDataFromFilePath(string themeFilePath)
    {
        var result = new ThemeData();

        var dataReaderArray = new NeoLemmixDataReader[]
        {
            new ThemeReader(result, _uniqueStringSet)
        };

        using var dataReaderList = new DataReaderList(themeFilePath, dataReaderArray);
        dataReaderList.ReadFile();

        return result;
    }

    private void ReadTerrainArchetypeData(string styleFolderPath)
    {
        var terrainFolderPath = Path.Combine(styleFolderPath, DefaultFileExtensions.TerrainFolderName);
        var terrainMetadataFilePaths = Helpers.GetFilePathsWithExtension(terrainFolderPath, NeoLemmixFileExtensions.TerrainFileExtension);
        var terrainPngFilePaths = Helpers.GetFilePathsWithExtension(terrainFolderPath, DefaultFileExtensions.PngFileExtension);

        _styleData.TerrainArchetypeDataLookup.EnsureCapacity(terrainPngFilePaths.Length);

        foreach (var pngFilePath in terrainPngFilePaths)
        {
            var correspondingTerrainMetadataFile = TryFindCorrespondingTerrainMetadataFile(pngFilePath, terrainMetadataFilePaths);
            if (correspondingTerrainMetadataFile is not null)
            {
                var newTerrainArchetypeData = ProcessTerrainMetadataFile(correspondingTerrainMetadataFile);
                _styleData.TerrainArchetypeDataLookup.Add(newTerrainArchetypeData.PieceIdentifier, newTerrainArchetypeData);
            }
            else
            {
                var pieceIdentifier = GetPieceIdentifier(pngFilePath);

                var trivialTerrainArchetypeData = TerrainArchetypeData.CreateTrivialTerrainArchetypeData(_styleData.Identifier, pieceIdentifier, pngFilePath);

                _styleData.TerrainArchetypeDataLookup.Add(pieceIdentifier, trivialTerrainArchetypeData);
            }
        }
    }

    private static string? TryFindCorrespondingTerrainMetadataFile(string terrainPngFilePath, string[] terrainMetadataFilePaths)
    {
        var pngFileNameSpan = Path.GetFileNameWithoutExtension(terrainPngFilePath.AsSpan());
        foreach (var terrainMetadataFilePath in terrainMetadataFilePaths)
        {
            var terrainMetadataFileNameSpan = Path.GetFileNameWithoutExtension(terrainMetadataFilePath.AsSpan());
            if (pngFileNameSpan.Equals(terrainMetadataFileNameSpan, StringComparison.Ordinal))
                return terrainMetadataFilePath;
        }

        return null;
    }

    private TerrainArchetypeData ProcessTerrainMetadataFile(string filePath)
    {
        var pieceIdentifier = GetPieceIdentifier(filePath);

        var terrainArchetypeDataReader = new TerrainArchetypeDataReader(filePath, _styleData.Identifier, pieceIdentifier);

        var dataReaderArray = new NeoLemmixDataReader[]
        {
            terrainArchetypeDataReader
        };

        using var dataReaderList = new DataReaderList(filePath, dataReaderArray);
        dataReaderList.ReadFile();

        return terrainArchetypeDataReader.CreateTerrainArchetypeData();
    }

    private PieceIdentifier GetPieceIdentifier(string filePath)
    {
        var fileNameSpan = Path.GetFileNameWithoutExtension(filePath.AsSpan());
        var fileNameString = _uniqueStringSet.GetUniqueStringInstance(fileNameSpan);
        return new PieceIdentifier(fileNameString);
    }

    private void ReadGadgetArchetypeData(string styleFolderPath)
    {
        var gadgetFolderPath = Path.Combine(styleFolderPath, DefaultFileExtensions.GadgetFolderName);
        var gadgetFilePaths = Helpers.GetFilePathsWithExtension(gadgetFolderPath, NeoLemmixFileExtensions.GadgetFileExtension);

        _styleData.GadgetArchetypeDataLookup.EnsureCapacity(gadgetFilePaths.Length);

        foreach (var filePath in gadgetFilePaths)
        {
            var newGadgetArchetypeData = ProcessGadgetFile(filePath);
            _styleData.GadgetArchetypeDataLookup.Add(newGadgetArchetypeData.PieceIdentifier, newGadgetArchetypeData);
        }
    }

    private GadgetArchetypeData ProcessGadgetFile(string filePath)
    {
        var pieceIdentifier = GetPieceIdentifier(filePath);

        var neoLemmixGadgetArchetypeData = new NeoLemmixGadgetArchetypeData(filePath, _styleData.Identifier, pieceIdentifier);

        var dataReaderArray = new NeoLemmixDataReader[]
        {
            new GadgetArchetypeDataReader(neoLemmixGadgetArchetypeData),
            new GadgetAnimationReader(neoLemmixGadgetArchetypeData)
        };

        using var dataReaderList = new DataReaderList(filePath, dataReaderArray);
        dataReaderList.ReadFile();

        return GadgetConverter.ConvertToGadgetArchetypeData(neoLemmixGadgetArchetypeData);
    }

    public void Dispose()
    {
    }
}
