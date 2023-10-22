using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public static class LevelFileTypeHandler
{
    public static ILevelReader GetLevelReaderForFileExtension(
        string fileExtension,
        RootDirectoryManager rootDirectoryManager)
    {
        return fileExtension.ToUpperInvariant() switch
        {
            ".NXLV" => new NxlvLevelReader(rootDirectoryManager),

            _ => throw new ArgumentException("Unknown file extension", nameof(fileExtension))
        };
    }
}