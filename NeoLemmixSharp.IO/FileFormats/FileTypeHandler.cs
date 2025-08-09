using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Reading.Levels;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat;
using NeoLemmixSharp.IO.Reading.Styles;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat;

namespace NeoLemmixSharp.IO.FileFormats;

public static class FileTypeHandler
{
    private readonly struct FileTypeAndFormat(FileType type, FileFormatType format)
    {
        public readonly FileType Type = type;
        public readonly FileFormatType Format = format;
    }

    private static readonly Dictionary<string, FileTypeAndFormat> FileTypeAndFormatLookup = new(8, StringComparer.OrdinalIgnoreCase)
    {
        { DefaultFileExtensions.LevelFileExtension, new(FileType.Level, FileFormatType.Default) },
        { DefaultFileExtensions.StyleFileExtension, new(FileType.Style, FileFormatType.Default) },

        { NeoLemmixFileExtensions.LevelFileExtension, new(FileType.Level, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.GadgetFileExtension, new(FileType.NeoLemmixGadget, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.TerrainFileExtension, new(FileType.NeoLemmixTerrain, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.ThemeFileExtension, new(FileType.Style, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.ConfigFileExtension, new(FileType.NeoLemmixConfig, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.ReplayFileExtension, new(FileType.Replay, FileFormatType.NeoLemmix) }
    };

    public static bool TryDetermineFileExtension(
        string filePath,
        out FileType fileType,
        out FileFormatType fileFormatType)
    {
        var fileExtension = Path.GetExtension(filePath.AsSpan());
        var fileTypeAndFormatAlternateLookup = FileTypeAndFormatLookup.GetAlternateLookup<ReadOnlySpan<char>>();
        var result = fileTypeAndFormatAlternateLookup.TryGetValue(fileExtension, out var typeAndFormat);

        fileType = typeAndFormat.Type;
        fileFormatType = typeAndFormat.Format;

        return result;
    }

    public static LevelData ReadLevel(string filePath)
    {
        var fileExtension = Path.GetExtension(filePath.AsSpan());
        if (fileExtension.IsEmpty)
            throw new ArgumentException($"No file extension specified: {filePath}");

        var fileTypeAndFormatAlternateLookup = FileTypeAndFormatLookup.GetAlternateLookup<ReadOnlySpan<char>>();
        if (!fileTypeAndFormatAlternateLookup.TryGetValue(fileExtension, out var typeAndFormat))
            throw new ArgumentException($"File extension not recognised: {fileExtension}");

        if (typeAndFormat.Type != FileType.Level)
            throw new ArgumentException($"File path does not point to a level: {filePath} -> {typeAndFormat.Type}");

        return typeAndFormat.Format switch
        {
            FileFormatType.Default => ReadLevel<DefaultLevelReader>(filePath),
            FileFormatType.NeoLemmix => ReadLevel<NeoLemmixLevelReader>(filePath),

            _ => Helpers.ThrowUnknownEnumValueException<FileFormatType, LevelData>(typeAndFormat.Format)
        };
    }

    private static LevelData ReadLevel<TReaderType>(string filePath)
        where TReaderType : ILevelReader<TReaderType>, allows ref struct
    {
        using var reader = TReaderType.Create(filePath);
        return reader.ReadLevel();
    }

    internal static StyleData ReadStyle(StyleFormatPair styleFormatPair)
    {
        return styleFormatPair.FileFormatType switch
        {
            FileFormatType.Default => ReadStyle<DefaultStyleReader>(styleFormatPair.StyleIdentifier),
            FileFormatType.NeoLemmix => ReadStyle<NeoLemmixStyleReader>(styleFormatPair.StyleIdentifier),

            _ => Helpers.ThrowUnknownEnumValueException<FileFormatType, StyleData>(styleFormatPair.FileFormatType)
        };
    }

    private static StyleData ReadStyle<TReaderType>(StyleIdentifier styleIdentifier)
        where TReaderType : IStyleReader<TReaderType>, allows ref struct
    {
        using var reader = TReaderType.Create(styleIdentifier);
        return reader.ReadStyle();
    }
}