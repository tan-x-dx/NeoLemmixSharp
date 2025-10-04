using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class ThemeData
{
    public Color Mask { get; set; } = Color.White;
    public Color Minimap { get; set; } = Color.White;
    public Color Background { get; set; } = Color.White;
    public Color OneWayArrows { get; set; } = Color.White;
    public Color PickupBorder { get; set; } = Color.White;
    public Color PickupInside { get; set; } = Color.White;

    public LemmingSpriteData LemmingSpriteData { get; set; } = null!;
}
