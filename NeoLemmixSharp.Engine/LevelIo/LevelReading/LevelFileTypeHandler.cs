using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelIo.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading;

public static class LevelFileTypeHandler
{
    private readonly struct FileTypeAndFormat(FileType type, FileFormatType format)
    {
        public readonly FileType Type = type;
        public readonly FileFormatType Format = format;
    }

    private static readonly Dictionary<string, FileTypeAndFormat> FileTypeAndFormatLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { DefaultFileExtensions.LevelFileExtension, new(FileType.Level, FileFormatType.Default) },
        { DefaultFileExtensions.LevelStyleExtension, new(FileType.Style, FileFormatType.Default) },

        { NeoLemmixFileExtensions.LevelFileExtension, new(FileType.Level,FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.GadgetFileExtension, new(FileType.NeoLemmixGadget, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.TerrainFileExtension, new(FileType.NeoLemmixTerrain, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.ThemeFileExtension, new(FileType.Style, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.ConfigFileExtension, new(FileType.NeoLemmixConfig, FileFormatType.NeoLemmix) },
        { NeoLemmixFileExtensions.ReplayFileExtension, new(FileType.Replay, FileFormatType.NeoLemmix) }
    };

    private static readonly Dictionary<string, Type> LevelFileExtensionLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { DefaultFileExtensions.LevelFileExtension, typeof(DefaultLevelReader) },
        { NeoLemmixFileExtensions.LevelFileExtension, typeof(NxlvLevelReader) }
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

    public static ILevelReader GetLevelReaderForFile(
        string filePath)
    {
        var fileExtension = Path.GetExtension(filePath.AsSpan());
        if (fileExtension.IsEmpty)
            throw new ArgumentException("No file extension specified!");

        var alternateLookup = LevelFileExtensionLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(fileExtension, out var levelReaderType))
            return (ILevelReader)Activator.CreateInstance(levelReaderType, filePath)!;

        throw new ArgumentException($"File extension not recognised: {fileExtension}");
    }
}