using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;
using NeoLemmixSharp.Menu.LevelPack;
using NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

namespace NeoLemmixSharp.Menu.LevelReading;

public static class LevelPackReader
{
    public static readonly string LevelsRootPath = Path.Combine(RootDirectoryManager.RootDirectory, NeoLemmixFileExtensions.LevelFolderName);

    public static IEnumerable<LevelPackData> TryReadLevelEntryFromFolder(string folderPath)
    {
        var files = Directory.GetFiles(folderPath);
        var shouldDoBasicSearchOnSubFolders = true;

        foreach (var filePath in files)
        {
            var fileExtension = Path.GetExtension(filePath.AsSpan());

            if (LevelFileTypeHandler.FileExtensionIsRecognised(fileExtension, out var fileType, out var fileFormatType))
            {
                switch (fileType)
                {
                    case FileType.Level:
                        yield return CreateLevelPackEntryForSingularLevel(filePath, fileFormatType);
                        break;

                    case FileType.NeoLemmixConfig:
                        shouldDoBasicSearchOnSubFolders = false;
                        yield return CreateLevelPackEntryForFolder(folderPath, FileFormatType.NeoLemmix);
                        break;
                }
            }
        }

        if (shouldDoBasicSearchOnSubFolders)
        {
            var subFolders = Directory.GetDirectories(folderPath);

            foreach (var subFolderEntry in subFolders.SelectMany(TryReadLevelEntryFromFolder))
            {
                yield return subFolderEntry;
            }
        }
    }

    private static LevelPackData CreateLevelPackEntryForSingularLevel(string filePath, FileFormatType fileFormatType)
    {
        var result = new LevelPackData
        {
            FileFormatType = fileFormatType,
            Music = new MusicData { },
            PackInfo = PackInfoData.Default,
        };

        var groupDatum = new LevelPackGroupData
        {
            FolderPath = filePath,
            FolderName = string.Empty,
            GroupName = string.Empty
        };

        result.GroupData.Add(groupDatum);
        result.PostViewMessages.AddRange(PostViewMessageData.DefaultMessages);

        return result;
    }

    private static LevelPackData CreateLevelPackEntryForFolder(string folderPath, FileFormatType fileFormatType)
    {
        if (fileFormatType == FileFormatType.NeoLemmix)
        {
            return NeoLemmixConfigReader.TryReadLevelPackData(folderPath);
        }

        return null!;
    }
}
