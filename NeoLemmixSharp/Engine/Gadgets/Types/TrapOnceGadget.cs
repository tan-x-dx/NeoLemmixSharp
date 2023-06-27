using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Gadgets.Types;

public sealed class TrapOnceGadget : Gadget
{
    public override GadgetType GadgetType { get; }
    public override int GadgetId { get; }
    public override bool CanActAsSolid { get; }
    public override bool CanActAsIndestructible { get; }
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