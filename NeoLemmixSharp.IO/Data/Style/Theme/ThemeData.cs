using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class ThemeData
{
    public required Color Mask { get; init; }
    public required Color Minimap { get; init; }
    public required Color Background { get; init; }
    public required Color OneWayArrows { get; init; }
    public required Color PickupBorder { get; init; }
    public required Color PickupInside { get; init; }

    public required LemmingSpriteData LemmingSpriteData { get; init; }
}
