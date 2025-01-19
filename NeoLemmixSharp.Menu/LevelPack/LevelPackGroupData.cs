namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class LevelPackGroupData
{
    public required string GroupName { get; init; }
    public required string FolderPath { get; init; }

    public required PackInfoData PackInfo { get; init; }
    public required List<string> MusicData { get; init; }
    public required List<PostViewMessageData> PostViewMessages { get; init; }
    public required List<LevelPackGroupData> SubGroups { get; init; }
    public required List<string> LevelFileNames { get; init; }
}
