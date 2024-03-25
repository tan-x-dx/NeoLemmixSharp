namespace NeoLemmixSharp.Common.Util;

public static class RootDirectoryManager
{
    public static void Initialise()
    {
        RootDirectory = ReadRootDirectoryForConfigFile();
    }

    public static string RootDirectory { get; private set; } = null!;

#if DEBUG
    private const string ConfigFileName = "DebugConfig.txt";
#endif

    /*
     * If this is your first time building in a dev environment, the program has probably thrown an error in this file!
     *
     * You need to create a file called DebugConfig.txt in the NeoLemmixSharp project directory.
     * Inside that file, you need to copy/paste the filepath to your NeoLemmix directory.
     */
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