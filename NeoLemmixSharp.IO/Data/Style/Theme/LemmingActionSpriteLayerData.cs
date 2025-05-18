namespace NeoLemmixSharp.IO.Data.Style.Theme;

public readonly struct LemmingActionSpriteLayerData(int layer, LemmingActionSpriteLayerColorType colorType)
{
    public readonly int Layer = layer;
    public readonly LemmingActionSpriteLayerColorType ColorType = colorType;
}
