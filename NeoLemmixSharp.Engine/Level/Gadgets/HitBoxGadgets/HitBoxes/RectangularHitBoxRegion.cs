using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class RectangularHitBoxRegion : IHitBoxRegion
{
    private readonly LevelRegion _region;

    public LevelRegion CurrentBounds => _region;

    public RectangularHitBoxRegion(
        int x,
        int y,
        int w,
        int h)
    {
        var position = new LevelPosition(x, y);
        var size = new LevelSize(w, h);
        _region = new LevelRegion(position, size);
    }

    public RectangularHitBoxRegion(
        LevelPosition p0,
        LevelPosition p1)
    {
        _region = new LevelRegion(p0, p1);
    }

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        var x0 = _region.X;
        var y0 = _region.Y;

        var x = levelPosition.X;
        var y = levelPosition.Y;

        var p1 = _region.GetBottomRight();
        var x1 = p1.X;
        var y1 = p1.Y;

        LevelScreen.HorizontalBoundaryBehaviour.NormaliseCoords(ref x0, ref x1, ref x);
        LevelScreen.VerticalBoundaryBehaviour.NormaliseCoords(ref y0, ref y1, ref y);

        return x0 <= x &&
               y0 <= y &&
               x < x1 &&
               y < y1;
    }
}
