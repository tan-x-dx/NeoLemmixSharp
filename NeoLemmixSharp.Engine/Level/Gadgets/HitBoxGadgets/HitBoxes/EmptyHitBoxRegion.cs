﻿using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class EmptyHitBoxRegion : HitBoxRegion
{
    public static readonly EmptyHitBoxRegion Instance = new();
    public override RectangularRegion CurrentBounds { get; } = default;

    public override bool ContainsPoint(Point levelPosition) => false;
    public override bool ContainsEitherPoint(Point p1, Point p2) => false;

    private EmptyHitBoxRegion()
    {
    }
}
