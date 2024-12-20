using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;
using NeoLemmixSharp.Menu.LevelPack;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class GroupConfigReader : NeoLemmixDataReader
{
    private readonly string _baseFolderPath;
    private readonly List<LevelPackGroupData> _groupData = [];

    private string? _folderName;
    private string? _groupName;

    public GroupConfigReader(string baseFolderPath) : base(string.Empty)
    {
        _baseFolderPath = baseFolderPath;

        RegisterTokenAction("BASE", DoNothing);
        RegisterTokenAction("$GROUP", OnEnterGroup);
        RegisterTokenAction("$RANK", OnEnterGroup);
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
        _folderName = null;
        _groupName = null;
    }

    private void ReadGroupName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _groupName = line.TrimAfterIndex(secondTokenIndex).ToString();
    }

    private void ReadFolder(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _folderName = line.TrimAfterIndex(secondTokenIndex).ToString();
    }

    private void OnExitGroup(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (_folderName is null)
            throw new InvalidOperationException("Missing folder name!");

        if (_groupName is null)
            _groupName = _folderName;

        var newLevelPackGroupData = new LevelPackGroupData
        {
            GroupName = _groupName,
            FolderPath = Path.Combine(_baseFolderPath, _folderName),

            LevelFileNames = []
        };

        _groupData.Add(newLevelPackGroupData);
    }

    public List<LevelPackGroupData> GetGroupData() => _groupData;
}
