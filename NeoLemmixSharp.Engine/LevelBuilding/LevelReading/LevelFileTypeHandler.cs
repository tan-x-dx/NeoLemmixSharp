using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class LevelFileTypeHandler
{
    private static readonly Dictionary<string, FileFormatType> FileFormatTypeLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { NeoLemmixFileExtensions.LevelFileExtension, FileFormatType.NeoLemmix },
        { NeoLemmixFileExtensions.GadgetFileExtension, FileFormatType.NeoLemmix },
        { NeoLemmixFileExtensions.TerrainFolderName, FileFormatType.NeoLemmix },
        { NeoLemmixFileExtensions.ThemeFileExtension, FileFormatType.NeoLemmix },
        { NeoLemmixFileExtensions.ConfigFileExtension, FileFormatType.NeoLemmix },
        { NeoLemmixFileExtensions.ReplayFileExtension, FileFormatType.NeoLemmix },

        { DefaultFileExtensions.LevelFileExtension, FileFormatType.Default }
    };

    private static readonly Dictionary<string, FileType> LevelFileTypeLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { NeoLemmixFileExtensions.LevelFileExtension, FileType.Level },
        { NeoLemmixFileExtensions.GadgetFileExtension, FileType.NeoLemmixGadget },
        { NeoLemmixFileExtensions.TerrainFolderName, FileType.NeoLemmixTerrain },
        { NeoLemmixFileExtensions.ThemeFileExtension, FileType.Style },
        { NeoLemmixFileExtensions.ConfigFileExtension, FileType.NeoLemmixConfig },
        { NeoLemmixFileExtensions.ReplayFileExtension, FileType.Replay },

        { DefaultFileExtensions.LevelFileExtension, FileType.Level }
    };

    private static readonly Dictionary<string, Type> LevelFileExtensionLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { NeoLemmixFileExtensions.LevelFileExtension, typeof(NxlvLevelReader) },
        { DefaultFileExtensions.LevelFileExtension, typeof(DefaultLevelReader) }
    };

    public static bool FileExtensionIsRecognised(
        ReadOnlySpan<char> fileExtension,
        out FileType fileType,
        out FileFormatType fileFormatType)
    {
        var fileExtensionAlternateLookup = LevelFileTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();
        var result = fileExtensionAlternateLookup.TryGetValue(fileExtension, out fileType);

        var fileFormatTypeAlternateLookup = FileFormatTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();
        fileFormatTypeAlternateLookup.TryGetValue(fileExtension, out fileFormatType);

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
        {
            var levelReader = Activator.CreateInstance(levelReaderType, filePath)!;
            return (ILevelReader)levelReader;
        }

        throw new ArgumentException($"File extension not recognised: {fileExtension}");
    }
}