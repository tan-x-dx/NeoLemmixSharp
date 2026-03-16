using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;
using Point = NeoLemmixSharp.Common.Point;

namespace NeoLemmixSharp.Menu.LevelEditor;

public static class LevelEditorConstants
{
    public const int ArrowKeyScrollDelta = 16;

    public const int LevelOuterBoundarySize = 128;

    /// <summary>
    /// Wait this number of frames before attempting a click/drag operation
    /// </summary>
    public const int CanvasClickDragDelay = 6;

    public static Point RenderOffset => new(LevelOuterBoundarySize, LevelOuterBoundarySize);
    public static Point InverseRenderOffset => new(-LevelOuterBoundarySize, -LevelOuterBoundarySize);

    public const uint CanvasBorderColourValue = 0xff5a5a5a;
    public static Color CanvasBorderColour => CanvasBorderColourValue.AsAbgrColor();
    public const int CanvasExtraSpaceBoundary = 16;

    public const int CanvasMinZoom = 1;
    public const int CanvasMaxZoom = 12;

    public const int BaseSpriteRenderSize = 80;
    public const int TileBorderSize = 2;
}
