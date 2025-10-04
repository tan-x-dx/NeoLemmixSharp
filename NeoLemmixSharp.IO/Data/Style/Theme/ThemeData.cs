using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class ThemeData
{
    public Color Mask { get; set; }
    public Color Minimap { get; set; }
    public Color Background { get; set; }
    public Color OneWayArrows { get; set; }
    public Color PickupBorder { get; set; }
    public Color PickupInside { get; set; }

    public LemmingSpriteData LemmingSpriteData { get; set; } = null!;
}
