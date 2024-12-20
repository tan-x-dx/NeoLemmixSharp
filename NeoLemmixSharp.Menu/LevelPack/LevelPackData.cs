using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class LevelPackData
{
    public string Title => PackInfo.Title;

    public required FileFormatType FileFormatType { get; init; }
    public required PackInfoData PackInfo { get; init; }
    public required MusicData? Music { get; init; }
    public List<PostViewMessageData> PostViewMessages { get; } = [];
    public List<LevelPackGroupData> GroupData { get; } = [];
}
