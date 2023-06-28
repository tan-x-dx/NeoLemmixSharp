using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Gadgets.Types;

public sealed class HatchGadget : Gadget
{
    private readonly LevelPosition _anchorPosition;

    // public override GadgetType GadgetType => GadgetType.Hatch;
    public override bool CanActAsSolid => false;
    public override bool CanActAsIndestructible => false;

    public HatchGadget(
        int gadgetId,
        Orientation orientation,
        LevelPosition anchorPosition)
        : base(gadgetId, orientation)
    {
        _anchorPosition = anchorPosition;
    }

    public override bool IsSolidToLemming(LevelPosition levelPosition, Lemming lemming) => false;
    public override bool IsIndestructibleToLemming(LevelPosition levelPosition, Lemming lemming) => false;

    public override bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation) => false;
}