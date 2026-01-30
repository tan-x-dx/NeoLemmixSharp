using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class ResizableRectangularHitBoxRegion : HitBoxRegion
{
    private readonly GadgetBounds _gadgetBounds;

    private readonly int _dx;
    private readonly int _dy;
    private readonly int _dw;
    private readonly int _dh;

    public ResizableRectangularHitBoxRegion(
        GadgetBounds gadgetBounds,
        int dx,
        int dy,
        int dw,
        int dh)
    {
        _gadgetBounds = gadgetBounds;
        _dx = dx;
        _dy = dy;
        _dw = dw;
        _dh = dh;
    }

    private int GetX() => _gadgetBounds.Position.X + _dx;
    private int GetY() => _gadgetBounds.Position.Y + _dy;
    private int GetWidth() => _gadgetBounds.Width + _dw - _dx;
    private int GetHeight() => _gadgetBounds.Height + _dh - _dy;

    public override RectangularRegion CurrentBounds => new(
        new Point(GetX(), GetY()),
        new Size(GetWidth(), GetHeight()));

    public override bool ContainsPoint(Point levelPosition)
    {
        var w = GetWidth();
        if (w <= 0)
            return false;

        var interval = new Interval(GetX(), w);
        if (!LevelScreen.HorizontalBoundaryBehaviour.IntervalContainsPoint(interval, levelPosition.X))
            return false;

        var h = GetHeight();
        if (h <= 0)
            return false;

        interval = new Interval(GetY(), h);
        return LevelScreen.VerticalBoundaryBehaviour.IntervalContainsPoint(interval, levelPosition.Y);
    }

    public override bool ContainsEitherPoint(Point p1, Point p2)
    {
        var w = GetWidth();
        if (w <= 0)
            return false;
        var h = GetHeight();
        if (h <= 0)
            return false;

        var horizontalInterval = new Interval(GetX(), w);
        var verticalInterval = new Interval(GetY(), h);

        return (LevelScreen.HorizontalBoundaryBehaviour.IntervalContainsPoint(horizontalInterval, p1.X) &&
                LevelScreen.VerticalBoundaryBehaviour.IntervalContainsPoint(verticalInterval, p1.Y)) ||
               (LevelScreen.HorizontalBoundaryBehaviour.IntervalContainsPoint(horizontalInterval, p2.X) &&
                LevelScreen.VerticalBoundaryBehaviour.IntervalContainsPoint(verticalInterval, p2.Y));
    }
}
