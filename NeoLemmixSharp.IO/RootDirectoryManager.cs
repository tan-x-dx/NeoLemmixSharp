using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;

namespace NeoLemmixSharp.IO;

public static class RootDirectoryManager
{
    public const string LemmingsFolderName = "lemmings";
    public const string LevelFolderName = "levels";
    public const string MusicFolderName = "music";
    public const string ReplayFolderName = "Replay";
    public const string SketchesFolderName = "sketches";
    public const string SoundFolderName = "sound";
    public const string StyleFolderName = "styles";

    public const string GadgetFolderName = "objects";
    public const string TerrainFolderName = "terrain";

    public static void Initialise()
    {
        if (RootDirectory is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(RootDirectoryManager)} more than once!");

        RootDirectory = ReadRootDirectoryForConfigFile();
        LevelFolderDirectory = Path.Combine(RootDirectory, LevelFolderName);
        MusicFolderDirectory = Path.Combine(RootDirectory, MusicFolderName);
        ReplayFolderDirectory = Path.Combine(RootDirectory, ReplayFolderName);
        SketchesFolderDirectory = Path.Combine(RootDirectory, SketchesFolderName);
        SoundFolderDirectory = Path.Combine(RootDirectory, SoundFolderName);
        StyleFolderDirectory = Path.Combine(RootDirectory, StyleFolderName);
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

    public static string GetFolderFilePath(this StyleIdentifier styleIdentifier) => Path.Combine(StyleFolderDirectory, styleIdentifier.ToString());

    public static string GetCorrespondingImageFile(string filePath)
    {
        return Path.ChangeExtension(filePath, DefaultFileExtensions.PngFileExtension);
    }

    public static string GetCorrespondingTerrainPngFilePath(StyleIdentifier styleIdentifier, PieceIdentifier pieceIdentifier)
    {
        var rootFilePath = Path.Combine(
            StyleFolderDirectory,
            styleIdentifier.ToString(),
            TerrainFolderName,
            pieceIdentifier.ToString());

        return GetCorrespondingImageFile(rootFilePath);
    }

    public static string GetCorrespondingGadgetPngFilePath(StyleIdentifier styleIdentifier, PieceIdentifier pieceIdentifier)
    {
        var rootFilePath = Path.Combine(
            StyleFolderDirectory,
            styleIdentifier.ToString(),
            GadgetFolderName,
            pieceIdentifier.ToString());

        return GetCorrespondingImageFile(rootFilePath);
    }

    public static string GetStyleFolderPath(StyleIdentifier styleIdentifier)
    {
        return Path.Combine(StyleFolderDirectory, styleIdentifier.ToString());
    }

    public static string GetStyleTerrainFolderPath(this StyleIdentifier styleIdentifier)
    {
        return Path.Combine(StyleFolderDirectory, styleIdentifier.ToString(), TerrainFolderName);
    }

    public static string GetStyleGadgetFolderPath(this StyleIdentifier styleIdentifier)
    {
        return Path.Combine(StyleFolderDirectory, styleIdentifier.ToString(), GadgetFolderName);
    }

    public static string GetStyleLemmingFolderPath(this StyleIdentifier styleIdentifier)
    {
        return Path.Combine(
            StyleFolderDirectory,
            styleIdentifier.ToString(),
            LemmingsFolderName);
    }

    public static ReadOnlySpan<string> GetFilePathsWithExtension(string folderPath, ReadOnlySpan<char> requiredFileExtension)
    {
        var allFiles = Directory.GetFiles(folderPath);
        var numberOfRelevantFiles = 0;

        foreach (var file in allFiles)
        {
            var fileExtensionSpan = Path.GetExtension(file.AsSpan());

            if (Helpers.StringSpansMatch(fileExtensionSpan, requiredFileExtension))
            {
                allFiles.At(numberOfRelevantFiles++) = file;
            }
        }

        // Clear the unused strings to encourage garbage collection
        var upperSpan = Helpers.CreateSpan(allFiles, numberOfRelevantFiles, allFiles.Length - numberOfRelevantFiles);
        upperSpan.Clear();

        return Helpers.CreateReadOnlySpan(allFiles, 0, numberOfRelevantFiles);
    }

    public static ReadOnlySpan<char> GetFullFilePathWithoutExtension(ReadOnlySpan<char> fullFilePath)
    {
        var extension = Path.GetExtension(fullFilePath);

        var indexOfExtension = fullFilePath.IndexOf(extension, StringComparison.Ordinal);
        return fullFilePath[..indexOfExtension];
    }
}
