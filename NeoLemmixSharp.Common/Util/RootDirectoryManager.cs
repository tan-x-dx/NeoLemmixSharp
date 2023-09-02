namespace NeoLemmixSharp.Common.Util;

public sealed class RootDirectoryManager
{
    public string RootDirectory { get; }

    public RootDirectoryManager()
    {
#if DEBUG
        RootDirectory = ReadRootDirectoryForConfigFile();
#else
        RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
    }

#if DEBUG

    private const string ConfigFileName = "DebugConfig.txt";

    private string ReadRootDirectoryForConfigFile()
    {
        var rootDirectory = Directory.GetCurrentDirectory();
        var configFilePath = Path.Combine(rootDirectory, ConfigFileName);

        return File.ReadAllText(configFilePath);
    }
#endif
}