using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class LevelFileTypeHandler
{
    public static bool FileExtensionIsValidLevelType(ReadOnlySpan<char> extension)
    {
        return extension is ".nxlv";
    }

    public static ILevelReader GetLevelReaderForFileExtension(
        ReadOnlySpan<char> fileExtension)
    {
        return fileExtension switch
        {
            ".nxlv" => new NxlvLevelReader(),

            _ => throw new ArgumentException("Unknown file extension", nameof(fileExtension))
        };
    }
}