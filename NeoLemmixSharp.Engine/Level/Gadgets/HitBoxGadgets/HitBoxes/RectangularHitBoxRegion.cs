using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class RectangularHitBoxRegion : IHitBoxRegion
{
    private readonly Region _region;

    public Region CurrentBounds => _region;

    public RectangularHitBoxRegion(
        int x,
        int y,
        int w,
        int h)
    {
        var position = new Point(x, y);
        var size = new Size(w, h);
        _region = new Region(position, size);
    }

    public RectangularHitBoxRegion(
        Point p0,
        Point p1)
    {
        _region = new Region(p0, p1);
    }

    public bool ContainsPoint(Point levelPosition)
    {
        return LevelScreen.RegionContainsPoint(_region, levelPosition);
    }

    public bool ContainsPoints(Point p1, Point p2)
    {
        return LevelScreen.RegionContainsPoints(_region, p1, p2);
    }
}
