using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class LemmingActionSpriteData
{
    public required int LemmingActionId { get; init; }
    public required Point AnchorPoint { get; init; }

    public required LemmingActionSpriteLayerData[] Layers { get; init; }
}
