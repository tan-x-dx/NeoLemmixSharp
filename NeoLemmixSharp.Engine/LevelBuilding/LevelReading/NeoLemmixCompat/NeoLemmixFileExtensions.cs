using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.ConfigReaders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public static class NeoLemmixFileExtensions
{
    public const string LevelFileExtension = ".nxlv";
    public const string GadgetFileExtension = ".nxmo";
    public const string TerrainFileExtension = ".nxmt";
    public const string ConfigFileExtension = ".nxmi";
    public const string ReplayFileExtension = ".nxrp";
    public const string ThemeFileExtension = ".nxtm";

    public const string LevelFolderName = "levels";
    public const string StyleFolderName = "styles";
    public const string GadgetFolderName = "objects";
    public const string TerrainFolderName = "terrain";

    private static readonly Dictionary<string, Type> ConfigFileTypeLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "title.nxmi", typeof(TitleConfigReader) },
        { "levels.nxmi", typeof(LevelConfigReader) },
        { "info.nxmi", typeof(InfoConfigReader) },
        { "music.nxmi", typeof(MusicConfigReader) },
        { "postview.nxmi", typeof(PostViewConfigReader) },
    };

    public static NeoLemmixDataReader? GetConfigReaderForConfigFile(string filePath)
    {
        var alternateLookup = ConfigFileTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        var fileName = Path.GetFileName(filePath.AsSpan());

        if (alternateLookup.TryGetValue(fileName, out var configReaderType))
        {
            var levelReader = Activator.CreateInstance(configReaderType, filePath)!;
            return (NeoLemmixDataReader)levelReader;
        }

        return null;
    }
}