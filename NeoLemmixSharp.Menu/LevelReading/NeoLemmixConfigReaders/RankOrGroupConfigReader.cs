using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Menu.LevelPack;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class RankOrGroupConfigReader : NeoLemmixDataReader
{
    private readonly string _baseFolderPath;
    private readonly List<RawData> _rawData = [];

    private RankOrGroupType? _rankOrGroupType = null;

    private string? _folderName;
    private string? _itemName;

    public RankOrGroupType Type => _rankOrGroupType!.Value;

    public RankOrGroupConfigReader(string baseFolderPath) : base(string.Empty)
    {
        _baseFolderPath = baseFolderPath;

        RegisterTokenAction("BASE", DoNothing);
        RegisterTokenAction("$GROUP", OnEnterGroup);
        RegisterTokenAction("$RANK", OnEnterRank);
        RegisterTokenAction("NAME", ReadGroupName);
        RegisterTokenAction("FOLDER", ReadFolder);
        RegisterTokenAction("$END", OnExitGroup);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token) => true;

    public override bool BeginReading(ReadOnlySpan<char> line) => true;

    private void DoNothing(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // Do nothing. Hee hoo
    }

    private void OnEnterGroup(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (_rankOrGroupType == RankOrGroupType.Rank)
            throw new InvalidOperationException("Invalid Rank or Group state!");

        _rankOrGroupType = RankOrGroupType.Group;
        _folderName = null;
        _itemName = null;
    }

    private void OnEnterRank(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (_rankOrGroupType == RankOrGroupType.Group)
            throw new InvalidOperationException("Invalid Rank or Group state!");

        _rankOrGroupType = RankOrGroupType.Rank;
        _folderName = null;
        _itemName = null;
    }

    private void ReadGroupName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _itemName = line.TrimAfterIndex(secondTokenIndex).ToString();
    }

    private void ReadFolder(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _folderName = line.TrimAfterIndex(secondTokenIndex).ToString();
    }

    private void OnExitGroup(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (_folderName is null)
            throw new InvalidOperationException("Missing folder name!");

        if (_itemName is null)
            _itemName = _folderName;

        var newLevelPackGroupData = new RawData
        {
            Name = _itemName,
            FolderPath = Path.Combine(_baseFolderPath, _folderName),
        };

        _rawData.Add(newLevelPackGroupData);
    }

    public List<LevelPackGroupData> GetGroupData()
    {
        return _rawData.ConvertAll(ToGroupData);
    }

    private static LevelPackGroupData ToGroupData(RawData input)
    {
        return new LevelPackGroupData
        {
            GroupName = input.Name,
            FolderPath = input.FolderPath,

            LevelFileNames = []
        };
    }

    public List<LevelPackRankData> GetRankData()
    {
        return _rawData.ConvertAll(ToRankData);
    }

    private static LevelPackRankData ToRankData(RawData input)
    {
        return new LevelPackRankData
        {
            RankName = input.Name,
            FolderPath = input.FolderPath
        };
    }

    public enum RankOrGroupType
    {
        Group,
        Rank
    }

    private sealed class RawData
    {
        public required string Name { get; init; }
        public required string FolderPath { get; init; }
    }
}
