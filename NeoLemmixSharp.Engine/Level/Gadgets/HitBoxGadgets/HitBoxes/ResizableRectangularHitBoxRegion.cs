using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class ResizableRectangularHitBoxRegion : IHitBoxRegion
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

    public RectangularRegion CurrentBounds => new(
        new Point(GetX(), GetY()),
        new Size(GetWidth(), GetHeight()));

    public bool ContainsPoint(Point levelPosition)
    {
        var w = GetWidth();
        var h = GetHeight();

        return w > 0 &&
               h > 0 &&
               LevelScreen.HorizontalBoundaryBehaviour.IntervalContainsPoint(new Interval(GetX(), w), levelPosition.X) &&
               LevelScreen.VerticalBoundaryBehaviour.IntervalContainsPoint(new Interval(GetY(), h), levelPosition.Y);
    }

    public bool ContainsPoints(Point p1, Point p2)
    {
        var w = GetWidth();
        var h = GetHeight();

        return w > 0 &&
               h > 0 &&
               ((LevelScreen.HorizontalBoundaryBehaviour.IntervalContainsPoint(new Interval(GetX(), w), p1.X) &&
                 LevelScreen.VerticalBoundaryBehaviour.IntervalContainsPoint(new Interval(GetY(), h), p1.Y)) ||
                (LevelScreen.HorizontalBoundaryBehaviour.IntervalContainsPoint(new Interval(GetX(), w), p1.X) &&
                 LevelScreen.VerticalBoundaryBehaviour.IntervalContainsPoint(new Interval(GetY(), h), p1.Y)));
    }
}
