using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalWrapViewPortBehaviour : IVerticalViewPortBehaviour
{
    private readonly int _levelHeightInPixels;

    public int ViewPortY { get; private set; }
    public int ViewPortHeight { get; private set; }
    public int ScreenY => 0;
    public int ScreenHeight { get; private set; }

    public RenderInterval[] VerticalRenderIntervals { get; private set; }

    public VerticalWrapViewPortBehaviour(int levelHeightInPixels)
    {
        _levelHeightInPixels = levelHeightInPixels;

        VerticalRenderIntervals = new RenderInterval[1];
    }

    public int NormaliseY(int y)
    {
        if (y < 0)
            return y + _levelHeightInPixels;

        if (y >= _levelHeightInPixels)
            return y - _levelHeightInPixels;

        return y;
    }

    public void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight)
    {
        ViewPortHeight = (windowHeight - 64) / scaleMultiplier;

        if (ViewPortHeight < _levelHeightInPixels)
        {
            ScreenHeight = ViewPortHeight * scaleMultiplier;
        }
        else
        {
            ScreenHeight = _levelHeightInPixels * scaleMultiplier;
            ViewPortHeight = _levelHeightInPixels;
        }
    }

    public void ScrollVertically(int dy)
    {
        ViewPortY += dy;
        if (ViewPortY < 0)
        {
            ViewPortY += _levelHeightInPixels;
        }
        else if (ViewPortY >= _levelHeightInPixels)
        {
            ViewPortY -= _levelHeightInPixels;
        }
    }

    public void RecalculateVerticalRenderIntervals(int scaleMultiplier)
    {
        var numberOfVerticalRenderIntervals = 0;

        var result = new RenderInterval[numberOfVerticalRenderIntervals];

       // return result;
    }

    public int GetNumberOfVerticalRepeats()
    {
        return Math.Clamp(1 + (ScreenHeight + ScreenY) / _levelHeightInPixels, 1, 5);
    }
}