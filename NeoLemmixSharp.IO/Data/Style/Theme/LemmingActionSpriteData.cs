using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class LemmingActionSpriteData
{
    private readonly LemmingActionSpriteLayerData[] _layers;
    public LemmingActionType LemmingActionType { get; }
    public Point AnchorPoint { get; }

    public ReadOnlySpan<LemmingActionSpriteLayerData> Layers => new(_layers);

    public LemmingActionSpriteData(LemmingActionType lemmingActionType, Point anchorPoint, LemmingActionSpriteLayerData[] layers)
    {
        _layers = layers;
        LemmingActionType = lemmingActionType;
        AnchorPoint = anchorPoint;
    }
}
