using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.Menu.LevelPack;

public sealed class LevelPackData
{
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required FileFormatType FileFormatType { get; init; }

    public required List<LevelPackGroupData> Groups { get; init; }
}
