﻿using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Gadgets;

public sealed class HatchGadget : Gadget
{
    private readonly LevelPosition _anchorPosition;

    public override GadgetType GadgetType => GadgetType.Hatch;
    public override int GadgetId { get; }
    public override bool CanActAsSolid => false;
    public override bool CanActAsIndestructible => false;

    public HatchGadget(int gadgetId, LevelPosition anchorPosition)
    {
        GadgetId = gadgetId;
        _anchorPosition = anchorPosition;
    }

    public override bool IsSolidToLemming(LevelPosition levelPosition, Lemming lemming) => false;
    public override bool IsIndestructibleToLemming(LevelPosition levelPosition, Lemming lemming) => false;

    public override bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation) => false;
}