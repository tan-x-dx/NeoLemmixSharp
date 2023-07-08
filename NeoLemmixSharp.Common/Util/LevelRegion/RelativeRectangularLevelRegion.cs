using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Common.Util.LevelRegion;

public sealed class RelativeRectangularLevelRegion : IRectangularLevelRegion
{
    private readonly IRectangularLevelRegion _anchorRegion;
    private readonly int _dx;
    private readonly int _dx1;
    private readonly int _dy;
    private readonly int _dy1;

    public int X => _anchorRegion.X + _dx;
    public int Y => _anchorRegion.Y + _dy;
    public int W => X1 - X;
    public int H => Y1 - Y;
    public int X1 => _anchorRegion.X1 + _dx1;
    public int Y1 => _anchorRegion.Y1 + _dy1;

    public LevelPosition TopLeft => new(X, Y);
    public LevelPosition BottomRight => new(X1, Y1);

    public RelativeRectangularLevelRegion(
        IRectangularLevelRegion anchorRegion,
        int dx,
        int dx1,
        int dy,
        int dy1)
    {
        _anchorRegion = anchorRegion;
        _dx = dx;
        _dx1 = dx1;
        _dy = dy;
        _dy1 = dy1;
    }

    public bool ContainsPoint(LevelPosition levelPosition) => X <= levelPosition.X &&
                                                              Y <= levelPosition.Y &&
                                                              levelPosition.X < X1 &&
                                                              levelPosition.Y < Y1;

    public Rectangle ToRectangle() => new(X, Y, W, H);

    public bool IsEmpty => W <= 0 || H <= 0;
}