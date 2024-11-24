using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class LevelFileTypeHandler
{
    private static readonly Dictionary<string, Type> FileTypeLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { NeoLemmixFileExtensions.LevelFileExtension, typeof(NxlvLevelReader) },
        { DefaultFileExtensions.LevelFileExtension, typeof(DefaultLevelReader) }
    };

    public static bool FileExtensionIsValidLevelType(ReadOnlySpan<char> fileExtension)
    {
        var alternateLookup = FileTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        return alternateLookup.ContainsKey(fileExtension);
    }

    public static ILevelReader GetLevelReaderForFileExtension(
        ReadOnlySpan<char> fileExtension)
    {
        if (fileExtension.IsEmpty)
            throw new ArgumentException("No file extension specified!");

        var alternateLookup = FileTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(fileExtension, out var levelReaderType))
        {
            var levelReader = Activator.CreateInstance(levelReaderType)!;
            return (ILevelReader)levelReader;
        }

        throw new ArgumentException($"File extension not recognised: {fileExtension}");
    }
}