using NeoLemmixSharp.IO.Reading.Levels;
using NeoLemmixSharp.Menu.LevelPack;
using NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;
using System.Diagnostics;

namespace NeoLemmixSharp.Menu.LevelReading;

public static class LevelPackReader
{
    public static IEnumerable<LevelPackData?> TryReadLevelEntryFromFolder(string folderPath)
    {
        var files = Directory.GetFiles(folderPath);

        // Need to do one sweep to see if we're inside of a NeoLemmix level pack folder
        // and then handle that separately
        foreach (var filePath in files)
        {
            if (!LevelFileTypeHandler.TryDetermineFileExtension(filePath, out var fileType, out var fileFormatType) ||
                fileType != FileType.NeoLemmixConfig ||
                fileFormatType != FileFormatType.NeoLemmix)
                continue;

            // At this point we've found a .nxmi file
            // Assume we're in a NeoLemmix level pack folder and deal with that separately
            yield return NeoLemmixConfigReader.TryCreateLevelPackData(folderPath);

            // This method is a recursive yield/return enumerable
            // Use a goto to break out of all loops
            goto Quit;
        }

        foreach (var filePath in files)
        {
            if (!LevelFileTypeHandler.TryDetermineFileExtension(filePath, out var fileType, out var fileFormatType))
                continue;

            // At this point, there may still be levels but these will be
            // standalone levels and will need to be processed as such

            if (fileType == FileType.Level)
                yield return CreateLevelPackEntryForSingularLevel(filePath, fileFormatType);
        }

        var subFolders = Directory.GetDirectories(folderPath);

        foreach (var subFolder in subFolders)
        {
            foreach (var lpd in TryReadLevelEntryFromFolder(subFolder))
                yield return lpd;
        }

        Quit:;
    }

    private static LevelPackData CreateLevelPackEntryForSingularLevel(string levelFilePath, FileFormatType fileFormatType)
    {
        var groupDatum = new LevelPackGroupData
        {
            FolderPath = levelFilePath,
            GroupName = string.Empty,
            PackInfo = PackInfoData.Default,
            MusicData = [],
            PostViewMessages = PostViewMessageData.DefaultMessages,
            SubGroups = [],
            LevelFileNames = [levelFilePath]
        };

        var result = new LevelPackData
        {
            Title = string.Empty,
            Author = string.Empty,
            FileFormatType = fileFormatType,
            Groups = [groupDatum],
        };

        return result;
    }

    private static LevelPackData? TryCreateLevelPackEntryForFolder(string folderPath, FileFormatType fileFormatType)
    {
        Debug.Assert(fileFormatType != FileFormatType.NeoLemmix);

        return null;
    }
}
