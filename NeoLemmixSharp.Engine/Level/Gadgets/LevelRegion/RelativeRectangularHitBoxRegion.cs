using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

public sealed class RelativeRectangularHitBoxRegion : IRectangularHitBoxRegion
{
    private readonly IRectangularHitBoxRegion _anchorRegion;
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

    public RelativeRectangularHitBoxRegion(
        IRectangularHitBoxRegion anchorRegion,
        int dx,
        int dy,
        int dx1,
        int dy1)
    {
        _anchorRegion = anchorRegion;
        _dx = dx;
        _dy = dy;
        _dx1 = dx1;
        _dy1 = dy1;
    }

    public bool ContainsPoint(LevelPosition levelPosition) => X <= levelPosition.X &&
                                                              Y <= levelPosition.Y &&
                                                              levelPosition.X < X1 &&
                                                              levelPosition.Y < Y1;

    public Rectangle ToRectangle() => new(X, Y, W, H);

    public override string ToString() => $"[({X} {Y}), W:{W} H:{H}]";
}