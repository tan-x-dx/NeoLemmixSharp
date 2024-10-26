﻿using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

public sealed class RectangularHitBoxRegion : IRectangularHitBoxRegion
{
    private int _w;
    private int _h;

    public int X { get; set; }
    public int Y { get; set; }
    public int W
    {
        get => _w;
        set => _w = value < 0 ? 0 : value;
    }
    public int H
    {
        get => _h;
        set => _h = value < 0 ? 0 : value;
    }

    public int X1 => X + W;
    public int Y1 => Y + H;

    public LevelPosition TopLeft => new(X, Y);
    public LevelPosition BottomRight => new(X1 - 1, Y1 - 1);

    public RectangularHitBoxRegion(int x, int y, int w, int h)
    {
        X = x;
        Y = y;
        W = w;
        H = h;
    }

    public RectangularHitBoxRegion(LevelPosition p0, LevelPosition p1)
    {
        if (p0.X < p1.X)
        {
            X = p0.X;
            W = 1 + p1.X - X;
        }
        else
        {
            X = p1.X;
            W = 1 + p0.X - X;
        }

        if (p0.Y < p1.Y)
        {
            Y = p0.Y;
            H = 1 + p1.Y - Y;
        }
        else
        {
            Y = p1.Y;
            H = 1 + p0.Y - Y;
        }
    }

    public bool ContainsPoint(LevelPosition levelPosition) => X <= levelPosition.X &&
                                                              Y <= levelPosition.Y &&
                                                              levelPosition.X < X1 &&
                                                              levelPosition.Y < Y1;

    public Rectangle ToRectangle() => new(X, Y, W, H);

    public override string ToString() => $"[({X} {Y}), W:{W} H:{H}]";
}