using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class LevelFileTypeHandler
{
    private static readonly Dictionary<string, Type> FileTypeLookup = new()
    {
        { NeoLemmixFileExtensions.LevelFileExtension, typeof(NxlvLevelReader) },
        { DefaultFileExtensions.LevelFileExtension, typeof(DefaultLevelReader) }
    };

    public static bool FileExtensionIsValidLevelType(ReadOnlySpan<char> fileExtension)
    {
        var spanLookup = FileTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        return spanLookup.ContainsKey(fileExtension);
    }

    public static ILevelReader GetLevelReaderForFileExtension(
        ReadOnlySpan<char> fileExtension)
    {
        if (fileExtension.IsEmpty)
            throw new ArgumentException("Missing file extension");

        var spanLookup = FileTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (spanLookup.TryGetValue(fileExtension, out var levelReaderType))
        {
            var levelReader = Activator.CreateInstance(levelReaderType)!;
            return (ILevelReader)levelReader;
        }

        throw new ArgumentException("Unknown file extension", nameof(fileExtension));
    }
}