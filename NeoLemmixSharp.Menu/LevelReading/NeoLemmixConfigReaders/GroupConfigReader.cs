using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.Menu.LevelReading.NeoLemmixConfigReaders;

public sealed class GroupConfigReader : NeoLemmixDataReader
{
    private readonly string _baseFolderPath;
    private readonly List<RawGroupData> _rawData = [];

    private string? _folderName;
    private string? _itemName;

    public bool FileIsCorrect { get; private set; } = true;

    public GroupConfigReader(string baseFolderPath) : base(string.Empty)
    {
        _baseFolderPath = baseFolderPath;

        SetNumberOfTokens(7);

        RegisterTokenAction("BASE", DoNothing);
        RegisterTokenAction("$GROUP", OnEnterGroup);
        RegisterTokenAction("$RANK", OnEnterGroup);
        RegisterTokenAction("NAME", ReadGroupName);
        RegisterTokenAction("FOLDER", ReadFolder);
        RegisterTokenAction("$END", OnExitGroup);

        RegisterTokenAction("LEVEL", OnReadLevel);
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
        _itemName = null;
    }

    private void ReadGroupName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _itemName = line[secondTokenIndex..].Trim().ToString();
    }

    private void ReadFolder(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _folderName = line[secondTokenIndex..].Trim().ToString();
    }

    private void OnExitGroup(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (_folderName is null)
            throw new InvalidOperationException("Missing folder name!");

        _itemName ??= _folderName;

        var newLevelPackGroupData = new RawGroupData(
            _itemName,
            Path.Combine(_baseFolderPath, _folderName));

        _rawData.Add(newLevelPackGroupData);
    }

    private void OnReadLevel(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        FileIsCorrect = false;
        _rawData.Clear();
    }

    public List<RawGroupData> GetRawGroupData() => _rawData;
}
