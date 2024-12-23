namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class LevelPackRankData
{
    public required string RankName { get; init; }
    public required string FolderPath { get; init; }

    public PackInfoData PackInfo { get; set; }
    public List<string> MusicData { get; set; }
    public List<PostViewMessageData> PostViewMessages { get; set; }
    public List<LevelPackGroupData> GroupData { get; set; }
}
