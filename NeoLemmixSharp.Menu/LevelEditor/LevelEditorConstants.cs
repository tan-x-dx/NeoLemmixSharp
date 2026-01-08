using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Menu.LevelEditor;

public static class LevelEditorConstants
{
    public const int ArrowKeyScrollDelta = 16;

    public const int LevelOuterBoundarySize = 128;

    public const uint CanvasBorderColourValue = 0xff696969;
    public static Color CanvasBorderColour => new(CanvasBorderColourValue);
    public const int CanvasExtraSpaceBoundary = 16;

    public const int CanvasMinZoom = 1;
    public const int CanvasMaxZoom = 12;

}
