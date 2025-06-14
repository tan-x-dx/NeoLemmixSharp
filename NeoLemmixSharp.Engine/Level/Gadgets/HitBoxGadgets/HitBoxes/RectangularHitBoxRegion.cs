﻿using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class RectangularHitBoxRegion : IHitBoxRegion
{
    private readonly RectangularRegion _region;

    public RectangularRegion CurrentBounds => _region;

    public RectangularHitBoxRegion(
        Point p0,
        Point p1)
    {
        _region = new RectangularRegion(p0, p1);
    }

    public bool ContainsPoint(Point levelPosition)
    {
        return LevelScreen.RegionContainsPoint(_region, levelPosition);
    }

    public bool ContainsEitherPoint(Point p1, Point p2)
    {
        return LevelScreen.RegionContainsEitherPoint(_region, p1, p2);
    }
}
