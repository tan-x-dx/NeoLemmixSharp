using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class LevelFileTypeHandler
{
    private static readonly IEqualityComparer<char> CharEqualityComparer = new CaseInvariantCharEqualityComparer();

    private static readonly Dictionary<string, Type> FileTypeLookup = new()
    {
        { NeoLemmixFileExtensions.LevelFileExtension, typeof(NxlvLevelReader) },
        { DefaultFileExtensions.LevelFileExtension, typeof(DefaultLevelReader) }
    };

    public static bool FileExtensionIsValidLevelType(ReadOnlySpan<char> fileExtension)
    {
        var canonicalFileExtension = GetCanonicalFileExtensionName(fileExtension);
        return canonicalFileExtension is not null;
    }

    public static ILevelReader GetLevelReaderForFileExtension(
        ReadOnlySpan<char> fileExtension)
    {
        var canonicalFileExtension = GetCanonicalFileExtensionName(fileExtension);
        if (canonicalFileExtension is null)
            throw new ArgumentException($"File extension not recognised: {fileExtension}");

        var levelReaderType = FileTypeLookup[canonicalFileExtension];
        var levelReader = Activator.CreateInstance(levelReaderType)!;
        return (ILevelReader)levelReader;
    }

    /// <summary>
    /// Helper method to avoid excess string allocations. There are a small number of
    /// const string file extensions. Return references to those consts where possible.
    /// </summary>
    /// <param name="fileExtension">The file extension to convert.</param>
    /// <returns>An allocation free string reference for the file extension, or null if the input was not recognised.</returns>
    /// <exception cref="ArgumentException">If the input is not a recognised file extension.</exception>
    private static string? GetCanonicalFileExtensionName(ReadOnlySpan<char> fileExtension)
    {
        ReadOnlySpan<string> allFileExtensions =
        [
            DefaultFileExtensions.LevelFileExtension,
            NeoLemmixFileExtensions.LevelFileExtension
        ];

        foreach (var canconicalFileExtension in allFileExtensions)
        {
            if (MemoryExtensions.SequenceEqual(fileExtension, canconicalFileExtension, CharEqualityComparer))
                return canconicalFileExtension;
        }

        return null;
    }
}