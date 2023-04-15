using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalWrapViewPortBehaviour : IVerticalViewPortBehaviour
{
    private readonly int _height;

    public int ViewPortY { get; private set; }
    public int ViewPortHeight { get; private set; }
    public int ScreenY => 0;
    public int ScreenHeight { get; private set; }
    public int NumberOfVerticalTilings { get; private set; }

    public VerticalWrapViewPortBehaviour(int height)
    {
        _height = height;
    }

    public int NormaliseY(int y)
    {
        if (y < 0)
            return y + _height;

        if (y >= _height)
            return y - _height;

        return y;
    }

    public void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight)
    {
        ViewPortHeight = (windowHeight - 64) / scaleMultiplier;

        if (ViewPortHeight < _height)
        {
            ScreenHeight = ViewPortHeight * scaleMultiplier;
        }
        else
        {
            ScreenHeight = _height * scaleMultiplier;
            ViewPortHeight = _height;
        }

        ScrollVertically(0);
    }

    public void ScrollVertically(int dy)
    {
        ViewPortY += dy;
        if (ViewPortY < 0)
        {
            ViewPortY += _height;
        }
        else if (ViewPortY >= _height)
        {
            ViewPortY -= _height;
        }
    }

    public int GetNumberOfVerticalRepeats()
    {
        return Math.Clamp(1 + (ScreenHeight + ScreenY) / _height, 1, 5);
    }
}