﻿using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class EmptyHitBoxRegion : IHitBoxRegion
{
    public static readonly EmptyHitBoxRegion Instance = new();
    public RectangularRegion CurrentBounds { get; } = default;

    public bool ContainsPoint(Point levelPosition) => false;
    public bool ContainsEitherPoint(Point p1, Point p2) => false;

    private EmptyHitBoxRegion()
    {
    }
}
