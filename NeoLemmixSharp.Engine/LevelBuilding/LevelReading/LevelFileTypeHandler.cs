using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class LevelFileTypeHandler
{
    private static readonly Dictionary<string, Type> FileTypeLookup = new()
    {
        { NeoLemmixFileExtensions.LevelFileExtension, typeof(NxlvLevelReader) }
    };

    public static bool FileExtensionIsValidLevelType(string? fileExtension)
    {
        return !string.IsNullOrWhiteSpace(fileExtension) && FileTypeLookup.ContainsKey(fileExtension);
    }

    public static ILevelReader GetLevelReaderForFileExtension(
        string? fileExtension)
    {
        if (string.IsNullOrWhiteSpace(fileExtension))
            throw new ArgumentException("Missing file extension");

        if (FileTypeLookup.TryGetValue(fileExtension, out var levelReaderType))
        {
            var levelReader = Activator.CreateInstance(levelReaderType)!;
            return (ILevelReader)levelReader;
        }

        throw new ArgumentException("Unknown file extension", nameof(fileExtension));
    }

    public static string? MatchLevelFileExtension(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return null;

        var filePathSpan = filePath.AsSpan();

        var i = filePathSpan.Length - 1;
        while (i >= 0)
        {
            if (filePathSpan[i] == '.')
            {
                break;
            }

            i--;
        }

        var subSpan = filePathSpan[i..];

        return subSpan switch
        {
            NeoLemmixFileExtensions.LevelFileExtension => NeoLemmixFileExtensions.LevelFileExtension,

            _ => null,
        };
    }
}