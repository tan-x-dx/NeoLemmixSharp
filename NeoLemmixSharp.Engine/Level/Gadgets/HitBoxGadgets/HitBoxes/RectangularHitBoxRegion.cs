using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class RectangularHitBoxRegion : HitBoxRegion
{
    private readonly RectangularRegion _region;

    public override bool IsTrivial() => false;
    public override RectangularRegion CurrentBounds => _region;

    public RectangularHitBoxRegion(
        Point p0,
        Point p1)
    {
        _region = new RectangularRegion(p0, p1);
    }

    public override bool ContainsPoint(Point levelPosition)
    {
        return LevelScreen.RegionContainsPoint(_region, levelPosition);
    }

    public override bool ContainsEitherPoint(Point p1, Point p2)
    {
        return LevelScreen.RegionContainsEitherPoint(_region, p1, p2);
    }
}
