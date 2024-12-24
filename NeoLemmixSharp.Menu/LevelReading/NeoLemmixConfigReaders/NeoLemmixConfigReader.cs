using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;
using NeoLemmixSharp.Menu.LevelPack;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public static class NeoLemmixConfigReader
{
    private enum GroupReadResult
    {
        Failure,
        Group,
        Levels
    }

    public static LevelPackData? TryCreateLevelPackData(string folderPath)
    {
        var groupReadResult = TryReadGroupDataFromFolder(
            folderPath,
            out var packData,
            out var groupData,
            out var levels);

        return groupReadResult switch
        {
            GroupReadResult.Group => new LevelPackData
            {
                Title = packData!.Title,
                Author = packData.Author,
                FileFormatType = FileFormatType.NeoLemmix,

                Groups = groupData!
            },

            GroupReadResult.Levels => new LevelPackData
            {
                Title = string.Empty,
                Author = string.Empty,
                FileFormatType = FileFormatType.NeoLemmix,

                Groups = [CreateGroupFromRawLevels(folderPath, levels)]
            },

            _ => null
        };
    }

    private static GroupReadResult TryReadGroupDataFromFolder(
        string folderPath,
        out PackInfoData? packData,
        out List<LevelPackGroupData>? groupData,
        out List<string> levelFilenames)
    {
        var files = Directory.GetFiles(folderPath);

        if (!TryReadPostViewData(files, out var postViewData))
        {
            postViewData = PostViewMessageData.DefaultMessages;
        }

        if (TryReadPackInfoData(files, out packData) &&
            TryReadRawGroupData(folderPath, files, out var rawGroupData))
        {
            // Nice case: This is an actual level group folder.
            // The subfolders will either contain subgroups,
            // or levels

            groupData = new List<LevelPackGroupData>(rawGroupData.Count);

            foreach (var rawGroupDatum in rawGroupData)
            {
                var subFolder = rawGroupDatum.FolderPath;

                var groupReadResult = TryReadGroupDataFromFolder(
                    subFolder,
                    out var subPackData,
                    out var subGroupData,
                    out var levelFileNames);

                if (groupReadResult == GroupReadResult.Group)
                {
                    groupData.Add(CreateGroup(rawGroupDatum, subPackData!, postViewData, subGroupData!));
                }
                else if (groupReadResult == GroupReadResult.Levels)
                {
                    groupData.Add(CreateGroup(rawGroupDatum, packData, postViewData, levelFileNames));
                }
            }

            levelFilenames = [];

            return GroupReadResult.Group;
        }

        var result = TryReadLevels(folderPath, files, out levelFilenames)
            ? GroupReadResult.Levels
            : GroupReadResult.Failure;

        groupData = null;
        return result;
    }

    private static LevelPackGroupData CreateGroup(
        RawGroupData rawGroupDatum,
        PackInfoData packInfoData,
        List<PostViewMessageData> postViewData,
        List<LevelPackGroupData> subGroupData)
    {
        var subFolder = rawGroupDatum.FolderPath;

        var files = Directory.GetFiles(subFolder);

        if (!TryReadMusicData(files, out var musicData))
        {
            musicData = [];
        }

        return new LevelPackGroupData
        {
            GroupName = rawGroupDatum.Name,
            FolderPath = subFolder,

            PackInfo = packInfoData,
            MusicData = musicData,
            PostViewMessages = postViewData,

            SubGroups = subGroupData,
            LevelFileNames = []
        };
    }

    private static LevelPackGroupData CreateGroup(
        RawGroupData rawGroupDatum,
        PackInfoData packData,
        List<PostViewMessageData> postViewData,
        List<string> levelFileNames)
    {
        return new LevelPackGroupData
        {
            GroupName = rawGroupDatum.Name,
            FolderPath = rawGroupDatum.FolderPath,

            PackInfo = packData,
            MusicData = [],
            PostViewMessages = postViewData,

            SubGroups = [],
            LevelFileNames = levelFileNames
        };
    }

    private static LevelPackGroupData CreateGroupFromRawLevels(
        string folderPath,
        List<string> levelFileNames)
    {
        return new LevelPackGroupData
        {
            GroupName = string.Empty,
            FolderPath = folderPath,

            PackInfo = PackInfoData.Default,
            MusicData = [],
            PostViewMessages = PostViewMessageData.DefaultMessages,
            SubGroups = [],
            LevelFileNames = levelFileNames
        };
    }

    private static bool TryReadPackInfoData(
        ReadOnlySpan<string> files,
        [MaybeNullWhen(false)] out PackInfoData packInfoData)
    {
        if (!TryFindNeoLemmixConfigFile(files, "info.nxmi", out var foundFilePath))
        {
            packInfoData = null;
            return false;
        }

        var infoConfigReader = new InfoConfigReader();
        using var dataReader = new DataReaderList(foundFilePath, [infoConfigReader]);

        dataReader.ReadFile();
        packInfoData = infoConfigReader.GetPackInfoData();
        return true;
    }

    private static bool TryReadMusicData(
        ReadOnlySpan<string> files,
        [MaybeNullWhen(false)] out List<string> musicData)
    {
        if (!TryFindNeoLemmixConfigFile(files, "music.nxmi", out var foundFilePath))
        {
            musicData = null;
            return false;
        }

        var musicConfigReader = new MusicConfigReader();
        using var dataReader = new DataReaderList(foundFilePath, [musicConfigReader]);

        dataReader.ReadFile();
        musicData = musicConfigReader.GetMusicData();
        return true;
    }

    private static bool TryReadPostViewData(
        ReadOnlySpan<string> files,
        [MaybeNullWhen(false)] out List<PostViewMessageData> postViewMessages)
    {
        if (!TryFindNeoLemmixConfigFile(files, "postview.nxmi", out var foundFilePath))
        {
            postViewMessages = null;
            return false;
        }

        var postViewConfigReader = new PostViewConfigReader();
        using var dataReader = new DataReaderList(foundFilePath, [postViewConfigReader]);

        dataReader.ReadFile();
        postViewMessages = postViewConfigReader.GetPostViewData();
        return true;
    }

    private static bool TryReadRawGroupData(
        string folderPath,
        ReadOnlySpan<string> files,
        [MaybeNullWhen(false)] out List<RawGroupData> groupData)
    {
        if (!TryFindNeoLemmixConfigFile(files, "levels.nxmi", out var foundFilePath))
        {
            groupData = null;
            return false;
        }

        var groupConfigReader = new GroupConfigReader(folderPath);
        using var dataReader = new DataReaderList(foundFilePath, [groupConfigReader]);

        dataReader.ReadFile();

        if (groupConfigReader.FileIsCorrect)
        {
            groupData = groupConfigReader.GetRawGroupData();
            return true;
        }

        groupData = null;
        return false;
    }

    private static bool TryReadLevels(
        string folderPath,
        ReadOnlySpan<string> files,
        out List<string> levelFileNames)
    {
        if (!TryFindNeoLemmixConfigFile(files, "levels.nxmi", out var foundFilePath))
            return TryReadRawLevels(folderPath, out levelFileNames);

        var levelReader = new LevelConfigReader();
        using var dataReader = new DataReaderList(foundFilePath, [levelReader]);

        dataReader.ReadFile();

        levelFileNames = levelReader.GetLevelFileNames();
        return true;
    }

    private static bool TryReadRawLevels(
        string folderPath,
        out List<string> levelFileNames)
    {
        var files = Directory.GetFiles(folderPath);

        levelFileNames = [];

        var neoLemmixFileFormat = NeoLemmixFileExtensions.LevelFileExtension.AsSpan();

        foreach (var file in files)
        {
            var fileExtension = Path.GetExtension(file.AsSpan());

            if (neoLemmixFileFormat.Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                levelFileNames.Add(Path.GetFileName(file));
            }
        }

        return levelFileNames.Count > 0;
    }

    private static bool TryFindNeoLemmixConfigFile(
        ReadOnlySpan<string> files,
        ReadOnlySpan<char> fileToLocate,
        [MaybeNullWhen(false)] out string foundFilePath)
    {
        foreach (var file in files)
        {
            var fileNamePlusExtension = Path.GetFileName(file.AsSpan());

            if (fileToLocate.Equals(fileNamePlusExtension, StringComparison.OrdinalIgnoreCase))
            {
                foundFilePath = file;
                return true;
            }
        }

        foundFilePath = null;
        return false;
    }
}
