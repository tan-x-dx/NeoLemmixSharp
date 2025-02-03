namespace NeoLemmixSharp.Common.Util;

public static class RootDirectoryManager
{
    public static void Initialise()
    {
        if (RootDirectory is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(RootDirectoryManager)} more than once!");

        RootDirectory = ReadRootDirectoryForConfigFile();
        LevelFolderDirectory = Path.Combine(RootDirectory, DefaultFileExtensions.LevelFolderName);
        MusicFolderDirectory = Path.Combine(RootDirectory, DefaultFileExtensions.MusicFolderName);
        ReplayFolderDirectory = Path.Combine(RootDirectory, DefaultFileExtensions.ReplayFolderName);
        SketchesFolderDirectory = Path.Combine(RootDirectory, DefaultFileExtensions.SketchesFolderName);
        SoundFolderDirectory = Path.Combine(RootDirectory, DefaultFileExtensions.SoundFolderName);
        StyleFolderDirectory = Path.Combine(RootDirectory, DefaultFileExtensions.StyleFolderName);
    }

    public static string RootDirectory { get; private set; } = null!;
    public static string LevelFolderDirectory { get; private set; } = null!;
    public static string MusicFolderDirectory { get; private set; } = null!;
    public static string ReplayFolderDirectory { get; private set; } = null!;
    public static string SketchesFolderDirectory { get; private set; } = null!;
    public static string SoundFolderDirectory { get; private set; } = null!;
    public static string StyleFolderDirectory { get; private set; } = null!;

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