using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class LevelPackData
{
    public string Title => PackInfo.Title;

    public required FileFormatType FileFormatType { get; init; }
    public required PackInfoData PackInfo { get; init; }
    public required List<string> MusicData { get; init; }
    public required List<PostViewMessageData> PostViewMessages { get; init; }
    public required List<LevelPackGroupData> GroupData { get; init; }
}
