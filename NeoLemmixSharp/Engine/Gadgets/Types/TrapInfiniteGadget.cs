using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Gadgets.Types;

public sealed class TrapInfiniteGadget : Gadget
{
    // public override GadgetType GadgetType { get; }
    public override bool CanActAsSolid { get; }
    public override bool CanActAsIndestructible { get; }

    public TrapInfiniteGadget(int gadgetId, Orientation orientation)
        : base(gadgetId, orientation)
    {
    }

    public override bool IsSolidToLemming(LevelPosition levelPosition, Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsIndestructibleToLemming(LevelPosition levelPosition, Lemming lemming)
    {
        throw new System.NotImplementedException();
    }

    public override bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation)
    {
        throw new System.NotImplementedException();
    }
}