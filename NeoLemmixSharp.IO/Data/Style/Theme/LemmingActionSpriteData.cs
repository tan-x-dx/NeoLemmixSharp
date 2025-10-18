using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class LemmingActionSpriteData
{
    private readonly LemmingActionSpriteLayerData[] _layers;
    public int LemmingActionId { get; }
    public Point AnchorPoint { get; }

    public ReadOnlySpan<LemmingActionSpriteLayerData> Layers => new(_layers);

    public LemmingActionSpriteData(int lemmingActionId, Point anchorPoint, LemmingActionSpriteLayerData[] layers)
    {
        _layers = layers;
        LemmingActionId = lemmingActionId;
        AnchorPoint = anchorPoint;
    }
}
