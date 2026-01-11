using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using Point = NeoLemmixSharp.Common.Point;

namespace NeoLemmixSharp.Menu.LevelEditor;

public static class LevelEditorConstants
{
    public const int ArrowKeyScrollDelta = 16;

    public const int LevelOuterBoundarySize = 128;

    public static Point RenderOffset => new(LevelOuterBoundarySize, LevelOuterBoundarySize);

    public const uint CanvasBorderColourValue = 0xff696969;
    public static Color CanvasBorderColour => new(CanvasBorderColourValue);
    public const int CanvasExtraSpaceBoundary = 16;

    public const int CanvasMinZoom = 1;
    public const int CanvasMaxZoom = 12;

    public static object? GetFoo(LevelData levelData, Point mousePosition)
    {
        foreach (var lemming in levelData.PrePlacedLemmingData)
        {
            var lemmingRectangle = GetBoundingBox(lemming);
            if (lemmingRectangle.Contains(mousePosition))
                return lemming;
        }

        foreach(var gadget in levelData.AllGadgetInstanceData)
        {
            if (gadget.GadgetRenderMode == Common.Enums.GadgetRenderMode.BehindTerrain)
                continue;

            var gadgetRectangle = GetBoundingBox(gadget);
            if(gadgetRectangle.Contains(mousePosition))
                return gadget;
        }

        foreach (var gadget in levelData.AllGadgetInstanceData)
        {
            if (gadget.GadgetRenderMode != Common.Enums.GadgetRenderMode.BehindTerrain)
                continue;

            var gadgetRectangle = GetBoundingBox(gadget);
            if (gadgetRectangle.Contains(mousePosition))
                return gadget;
        }

        return null;
    }

    private static RectangularRegion GetBoundingBox(LemmingInstanceData lemming)
    {
        var lemmingAction = LemmingAction.GetActionOrDefault(lemming.InitialLemmingActionId);

        return lemmingAction.GetLemmingBounds(lemming.Orientation, lemming.FacingDirection, lemming.Position);
    }

    private static RectangularRegion GetBoundingBox(GadgetInstanceData gadget)
    {
        /*    var x = gadget.SpecificationData.

            return lemmingAction.GetLemmingBounds(lemming.Orientation, lemming.FacingDirection, lemming.Position);*/

        return default;
    }
}
