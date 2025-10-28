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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat;

internal readonly ref struct NeoLemmixStyleReader : IStyleReader<NeoLemmixStyleReader>
{
    private readonly StyleData _styleData;
    private readonly UniqueStringSet _uniqueStringSet;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NeoLemmixStyleReader Create(StyleIdentifier styleIdentifier) => new(styleIdentifier);

    private NeoLemmixStyleReader(StyleIdentifier styleIdentifier)
    {
        _styleData = new StyleData(styleIdentifier, FileFormats.FileFormatType.NeoLemmix);
    }

    public StyleData ReadStyle()
    {
        var styleFolderPath = _styleData.Identifier.GetFolderFilePath();

        // Reuse the array where possible
        var dataReaderArray = new NeoLemmixDataReader[1];

        _styleData.ThemeData = ReadThemeData(styleFolderPath, dataReaderArray);

        ReadTerrainArchetypeData(styleFolderPath, dataReaderArray);
        ReadGadgetArchetypeData(styleFolderPath);

        return _styleData;
    }

    private static ThemeData ReadThemeData(string styleFolderPath, NeoLemmixDataReader[] dataReaderArray)
    {
        var themeDataFilePaths = GetFilePaths(
            styleFolderPath,
            NeoLemmixFileExtensions.ThemeFileExtension.AsSpan());

        if (themeDataFilePaths.Count == 1)
            return ReadThemeDataFromFilePath(themeDataFilePaths[0], dataReaderArray);

        return StyleCache.DefaultStyleData.ThemeData;
    }

    private static ThemeData ReadThemeDataFromFilePath(string themeFilePath, NeoLemmixDataReader[] dataReaderArray)
    {
        var result = new ThemeData();

        dataReaderArray[0] = new ThemeReader(result);

        using var dataReaderList = new DataReaderList(themeFilePath, dataReaderArray);
        dataReaderList.ReadFile();

        return result;
    }

    private void ReadTerrainArchetypeData(string styleFolderPath, NeoLemmixDataReader[] dataReaderArray)
    {
        var terrainFilePaths = GetFilePaths(
            Path.Combine(styleFolderPath, DefaultFileExtensions.TerrainFolderName),
            NeoLemmixFileExtensions.TerrainFileExtension.AsSpan());

        foreach (var filePath in terrainFilePaths)
        {
            var newTerrainArchetypeData = ProcessTerrainFile(filePath, dataReaderArray);
            _styleData.TerrainArchetypeDataLookup.Add(newTerrainArchetypeData.PieceIdentifier, newTerrainArchetypeData);
        }
    }

    private TerrainArchetypeData ProcessTerrainFile(string filePath, NeoLemmixDataReader[] dataReaderArray)
    {
        var pieceIdentifier = new PieceIdentifier(Path.GetFileNameWithoutExtension(filePath));

        var terrainArchetypeDataReader = new TerrainArchetypeDataReader(filePath, _styleData.Identifier, pieceIdentifier);
        dataReaderArray[0] = terrainArchetypeDataReader;

        using var dataReaderList = new DataReaderList(filePath, dataReaderArray);
        dataReaderList.ReadFile();

        return terrainArchetypeDataReader.CreateTerrainArchetypeData();
    }

    private void ReadGadgetArchetypeData(string styleFolderPath)
    {
        var gadgetFilePaths = GetFilePaths(
            Path.Combine(styleFolderPath, DefaultFileExtensions.GadgetFolderName),
            NeoLemmixFileExtensions.GadgetFileExtension.AsSpan());

        foreach (var filePath in gadgetFilePaths)
        {
            var newGadgetArchetypeData = ProcessGadgetFile(filePath);
            _styleData.GadgetArchetypeDataLookup.Add(newGadgetArchetypeData.PieceIdentifier, newGadgetArchetypeData);
        }
    }

    private GadgetArchetypeData ProcessGadgetFile(string filePath)
    {
        var pieceIdentifier = new PieceIdentifier(Path.GetFileNameWithoutExtension(filePath));

        var neoLemmixGadgetArchetypeData = new NeoLemmixGadgetArchetypeData(_styleData.Identifier, pieceIdentifier);

        var dataReaderArray = new NeoLemmixDataReader[]
        {
            new GadgetArchetypeDataReader(neoLemmixGadgetArchetypeData),
            new PrimaryAnimationReader(neoLemmixGadgetArchetypeData),
            new SecondaryAnimationReader(neoLemmixGadgetArchetypeData, _uniqueStringSet)
        };

        using var dataReaderList = new DataReaderList(filePath, dataReaderArray);
        dataReaderList.ReadFile();

        return GadgetConverter.ConvertToGadgetArchetypeData(neoLemmixGadgetArchetypeData);
    }

    private static List<string> GetFilePaths(string folderPath, ReadOnlySpan<char> requiredFileExtension)
    {
        var allFiles = Directory.GetFiles(folderPath);
        var relevantFilePaths = new List<string>();

        foreach (var file in allFiles)
        {
            var span = file.AsSpan();
            var fileExtension = Path.GetExtension(span);

            if (requiredFileExtension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                relevantFilePaths.Add(file);
            }
        }

        return relevantFilePaths;
    }

    public void Dispose()
    {
    }
}
