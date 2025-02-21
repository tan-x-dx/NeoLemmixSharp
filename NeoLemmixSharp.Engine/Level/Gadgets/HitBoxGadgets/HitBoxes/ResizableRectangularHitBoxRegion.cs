﻿using NeoLemmixSharp.Common.Util;

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

    private int GetX() => _gadgetBounds.X + _dx;
    private int GetY() => _gadgetBounds.Y + _dy;
    private int GetWidth() => _gadgetBounds.Width + _dw - _dx;
    private int GetHeight() => _gadgetBounds.Height + _dh - _dy;

    public LevelRegion CurrentBounds => new(
        new LevelPosition(GetX(), GetY()),
        new LevelSize(GetWidth(), GetHeight()));

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        var x = GetX();
        var y = GetY();

        var w = GetWidth();
        var h = GetHeight();

        return w > 0 &&
               h > 0 &&
               LevelScreen.HorizontalBoundaryBehaviour.IsInRange(levelPosition.X, x, x + w) &&
               LevelScreen.VerticalBoundaryBehaviour.IsInRange(levelPosition.Y, y, y + h);
    }
}
