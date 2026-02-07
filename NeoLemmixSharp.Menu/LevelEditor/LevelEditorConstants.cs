using Microsoft.Xna.Framework;
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

    public const uint CanvasBorderColourValue = 0xff696969;
    public static Color CanvasBorderColour => new(CanvasBorderColourValue);
    public const int CanvasExtraSpaceBoundary = 16;

    public const int CanvasMinZoom = 1;
    public const int CanvasMaxZoom = 12;
}
