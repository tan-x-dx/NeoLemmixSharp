using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat;

internal readonly ref struct NeoLemmixStyleReader : IStyleReader<NeoLemmixStyleReader>
{
    private readonly StyleData _data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NeoLemmixStyleReader Create(StyleIdentifier styleIdentifier) => new(styleIdentifier);

    private NeoLemmixStyleReader(StyleIdentifier styleIdentifier)
    {
        _data = new StyleData(styleIdentifier, FileFormats.FileFormatType.NeoLemmix);
    }

    public StyleData ReadStyle()
    {
        var styleFolderPath = _data.Identifier.GetFolderFilePath();

        _data.ThemeData = ReadThemeData(styleFolderPath);

        return _data;
    }

    private ThemeData ReadThemeData(string styleFolderPath)
    {
        if (TryLocateThemeFile(styleFolderPath, out var themeFilePath))
            return ReadThemeDataFromFilePath(themeFilePath);

        return StyleCache.DefaultStyleData.ThemeData;
    }

    private static bool TryLocateThemeFile(string styleFolderPath, [MaybeNullWhen(false)] out string foundFilePath)
    {
        var files = Directory.GetFiles(styleFolderPath);

        foreach (var filePath in files)
        {
            var fileExtension = Path.GetExtension(filePath.AsSpan());

            if (fileExtension.Equals(NeoLemmixFileExtensions.ThemeFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                foundFilePath = filePath;
                return true;
            }
        }

        foundFilePath = null;
        return false;
    }

    private ThemeData ReadThemeDataFromFilePath(string themeFilePath)
    {
        var result = new ThemeData();

        var dataReaders = new NeoLemmixDataReader[]
        {
            new ThemeReader(result),
        };

        using var dataReaderList = new DataReaderList(themeFilePath, dataReaders);
        dataReaderList.ReadFile();

        return result;
    }

    private void ReadTerrainArchetypeData()
    {

    }

    private void ReadGadgetArchetypeData()
    {

    }

    public void Dispose()
    {
    }
}
