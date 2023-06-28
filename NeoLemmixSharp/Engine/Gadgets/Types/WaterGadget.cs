using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;
using NeoLemmixSharp.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Gadgets.Types;

public sealed class WaterGadget : Gadget
{
    private readonly RectangularLevelRegion _renderClip;
    private readonly RectangularLevelRegion _hitBox;

    // public override GadgetType GadgetType => GadgetType.Water;
    public override bool CanActAsSolid => false;
    public override bool CanActAsIndestructible => false;

    public WaterGadget(
        int gadgetId,
        Orientation orientation,
        RectangularLevelRegion renderClip,
        RectangularLevelRegion hitBox)
        : base(gadgetId, orientation)
    {
        _renderClip = renderClip;
        _hitBox = hitBox;
    }

    public override bool IsSolidToLemming(LevelPosition levelPosition, Lemming lemming) => false;
    public override bool IsIndestructibleToLemming(LevelPosition levelPosition, Lemming lemming) => false;

    public override bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation)
    {
        var offset = levelPosition - _hitBox.TopLeft;

        return _hitBox.ContainsPoint(offset) ||
               _hitBox.ContainsPoint(orientation.MoveUp(offset, 1));
    }
}