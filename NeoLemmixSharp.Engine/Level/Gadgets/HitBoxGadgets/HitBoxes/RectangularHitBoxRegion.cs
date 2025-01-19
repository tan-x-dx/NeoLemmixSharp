using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public sealed class RectangularHitBoxRegion : IHitBoxRegion
{
    private readonly LevelPosition _position;
    private readonly LevelSize _size;

    public LevelPosition Offset => _position;
    public LevelSize BoundingBoxDimensions => _size;

    public RectangularHitBoxRegion(
        int x,
        int y,
        int w,
        int h)
    {
        _position = new LevelPosition(x, y);
        _size = new LevelSize(w, h);
    }

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        var x0 = _position.X;
        var y0 = _position.Y;

        var x = levelPosition.X;
        var y = levelPosition.Y;

        var x1 = _position.X + _size.W;
        var y1 = _position.Y + _size.H;

        LevelScreen.HorizontalBoundaryBehaviour.NormaliseCoords(ref x0, ref x1, ref x);
        LevelScreen.VerticalBoundaryBehaviour.NormaliseCoords(ref y0, ref y1, ref y);

        return x0 <= x &&
               y0 <= y &&
               x < x1 &&
               y < y1;
    }
}
