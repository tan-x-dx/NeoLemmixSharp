namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class LevelPackGroupData
{
    public required string GroupName { get; init; }
    public required string FolderPath { get; init; }

    public required List<string> LevelFileNames { get; init; }
}
