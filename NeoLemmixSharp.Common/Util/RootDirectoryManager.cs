namespace NeoLemmixSharp.Common.Util;

public static class RootDirectoryManager
{
    public static void Initialise()
    {
        RootDirectory = ReadRootDirectoryForConfigFile();
    }

    public static string RootDirectory { get; private set; } = null!;

    private const string ConfigFileName = "DebugConfig.txt";

    private static string ReadRootDirectoryForConfigFile()
    {
#if DEBUG
        var rootDirectory = Directory.GetCurrentDirectory();
        var configFilePath = Path.Combine(rootDirectory, ConfigFileName);

        return File.ReadAllText(configFilePath);
#else
        return AppDomain.CurrentDomain.BaseDirectory;
#endif
    }
}