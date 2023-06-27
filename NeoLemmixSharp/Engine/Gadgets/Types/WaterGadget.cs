using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Gadgets.Types;

public sealed class WaterGadget : Gadget
{
    private readonly RectangularLevelRegion _levelRegion;

    // public override GadgetType GadgetType => GadgetType.Water;
    public override int GadgetId { get; }
    public override bool CanActAsSolid => false;
    public override bool CanActAsIndestructible => false;

    public WaterGadget(int gadgetId, RectangularLevelRegion levelRegion)
    {
        GadgetId = gadgetId;
        _levelRegion = levelRegion;
    }

    public override bool IsSolidToLemming(LevelPosition levelPosition, Lemming lemming) => false;
    public override bool IsIndestructibleToLemming(LevelPosition levelPosition, Lemming lemming) => false;

    public override bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation)
    {
        return _levelRegion.ContainsPoint(levelPosition); // Water objects do not care about orientation;
    }
}