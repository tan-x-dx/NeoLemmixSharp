using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;
using NeoLemmixSharp.Menu.LevelPack;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public static class NeoLemmixConfigReader
{
    public static LevelPackData TryReadLevelPackData(string folderPath)
    {
        var files = Directory.GetFiles(folderPath);

        var packInfoData = TryReadPackInfoData(files);
        var musicData = TryReadMusicData(files);
        var postViewData = TryReadPostViewData(files);
        var groupData = TryReadGroupData(folderPath, files);

        return new LevelPackData
        {
            FileFormatType = FileFormatType.NeoLemmix,

            PackInfo = packInfoData,
            MusicData = musicData,
            PostViewMessages = postViewData,
            GroupData = groupData
        };
    }

    private static PackInfoData TryReadPackInfoData(ReadOnlySpan<string> files)
    {
        if (!TryFindNeoLemmixConfigFile(files, "info.nxmi", out var foundFilePath))
            return PackInfoData.Default;

        var infoConfigReader = new InfoConfigReader();
        using var dataReader = new DataReaderList(foundFilePath, [infoConfigReader]);

        dataReader.ReadFile();
        return infoConfigReader.GetPackInfoData();
    }

    private static List<string> TryReadMusicData(ReadOnlySpan<string> files)
    {
        if (!TryFindNeoLemmixConfigFile(files, "music.nxmi", out var foundFilePath))
            return [];

        var musicConfigReader = new MusicConfigReader();
        using var dataReader = new DataReaderList(foundFilePath, [musicConfigReader]);

        dataReader.ReadFile();
        return musicConfigReader.GetMusicData();
    }

    private static List<PostViewMessageData> TryReadPostViewData(ReadOnlySpan<string> files)
    {
        if (!TryFindNeoLemmixConfigFile(files, "postview.nxmi", out var foundFilePath))
            return PostViewMessageData.DefaultMessages;

        var postViewConfigReader = new PostViewConfigReader();
        using var dataReader = new DataReaderList(foundFilePath, [postViewConfigReader]);

        dataReader.ReadFile();
        return postViewConfigReader.GetPostViewData();
    }

    private static List<LevelPackGroupData> TryReadGroupData(string folderPath, ReadOnlySpan<string> files)
    {
        if (!TryFindNeoLemmixConfigFile(files, "levels.nxmi", out var foundFilePath))
            return TryReadRawLevels(folderPath);

        var groupConfigReader = new GroupConfigReader(folderPath);
        using var dataReader = new DataReaderList(foundFilePath, [groupConfigReader]);

        dataReader.ReadFile();
        var groups = groupConfigReader.GetGroupData();

        foreach (var group in groups)
        {
            TryReadGroupLevelData(group);
        }

        return groups;
    }

    private static List<LevelPackGroupData> TryReadRawLevels(string folderPath)
    {
        var subFolders = Directory.GetDirectories(folderPath);
        var result = new List<LevelPackGroupData>();

        foreach (var subFolder in subFolders)
        {
            var group = new LevelPackGroupData
            {
                FolderPath = subFolder,
                GroupName = subFolder,

                LevelFileNames = []
            };

            TryReadGroupLevelData(group);
            if (group.LevelFileNames.Count > 0)
            {
                result.Add(group);
            }
        }

        return result;
    }

    private static void TryReadGroupLevelData(LevelPackGroupData group)
    {
        var files = Directory.GetFiles(group.FolderPath);

        if (TryFindNeoLemmixConfigFile(files, "levels.nxmi", out var foundFilePath))
        {
            var levelConfigReader = new LevelConfigReader();
            using var dataReader = new DataReaderList(foundFilePath, [levelConfigReader]);

            dataReader.ReadFile();
            group.LevelFileNames.AddRange(levelConfigReader.GetLevelFileNames());
            return;
        }

        foreach (var file in files)
        {
            var fileExtension = Path.GetExtension(file.AsSpan());
            if (!LevelFileTypeHandler.FileExtensionIsRecognised(fileExtension, out var fileType, out var fileFormatType) ||
                fileType != FileType.Level ||
                fileFormatType != FileFormatType.NeoLemmix)
                continue;

            var fileNamePlusExtension = Path.GetFileName(file);
            group.LevelFileNames.Add(fileNamePlusExtension);
        }
    }

    private static bool TryFindNeoLemmixConfigFile(ReadOnlySpan<string> files, ReadOnlySpan<char> fileNameToLocate, [MaybeNullWhen(false)] out string foundFilePath)
    {
        foreach (var file in files)
        {
            var fileNamePlusExtension = Path.GetFileName(file.AsSpan());

            if (fileNameToLocate.SequenceEqual(fileNamePlusExtension, Helpers.CaseInvariantCharEqualityComparer))
            {
                foundFilePath = file;
                return true;
            }
        }

        foundFilePath = null;
        return false;
    }
}
