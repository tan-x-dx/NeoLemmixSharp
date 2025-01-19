using NeoLemmixSharp.Common.Util;

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

    private int X => _gadgetBounds.X + _dx;
    private int Y => _gadgetBounds.Y + _dy;
    private int Width => _gadgetBounds.Width + _dw - _dx;
    private int Height => _gadgetBounds.Height + _dh - _dy;

    public LevelPosition Offset => new(X, Y);
    public LevelSize BoundingBoxDimensions => new(Width, Height);

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        var w = Width;
        var h = Height;

        if (w <= 0 ||
            h <= 0)
            return false;

        var x0 = X;
        var y0 = Y;

        var x1 = x0 + w;
        var y1 = y0 + h;

        var x = levelPosition.X;
        var y = levelPosition.Y;

        LevelScreen.HorizontalBoundaryBehaviour.NormaliseCoords(ref x0, ref x1, ref x);
        LevelScreen.VerticalBoundaryBehaviour.NormaliseCoords(ref y0, ref y1, ref y);

        return x0 <= x &&
               y0 <= y &&
               x < x1 &&
               y < y1;
    }
}
