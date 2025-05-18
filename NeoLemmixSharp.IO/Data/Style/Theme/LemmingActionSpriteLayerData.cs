using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public readonly struct LemmingActionSpriteLayerData(int layer, Color color)
{
    public readonly int Layer = layer;
    public readonly Color Color = color;
}
