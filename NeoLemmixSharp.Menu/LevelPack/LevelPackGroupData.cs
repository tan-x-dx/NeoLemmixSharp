namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class LevelPackGroupData
{
    public required string GroupName { get; init; }
    public required string FolderName { get; init; }
    public required string FolderPath { get; init; }

    public List<string> LevelFilePaths { get; } = [];
}
