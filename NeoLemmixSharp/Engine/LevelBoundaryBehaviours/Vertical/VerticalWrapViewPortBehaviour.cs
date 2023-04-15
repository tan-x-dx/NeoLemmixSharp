using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalWrapViewPortBehaviour : IVerticalViewPortBehaviour
{
    private readonly int _levelHeightInPixels;

    private int _numberOfVerticalRenderIntervals = 1;

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
        var previousNumberOfVerticalRenderIntervals = _numberOfVerticalRenderIntervals;
        _numberOfVerticalRenderIntervals = Math.Clamp(1 + (ViewPortY + ViewPortHeight - 1) / _levelHeightInPixels, 1, 5);

        if (_numberOfVerticalRenderIntervals != previousNumberOfVerticalRenderIntervals)
        {
            VerticalRenderIntervals = new RenderInterval[_numberOfVerticalRenderIntervals];
        }

        if (_numberOfVerticalRenderIntervals == 1)
        {
            VerticalRenderIntervals[0] = new RenderInterval(ViewPortY, ViewPortHeight, ScreenY, ScreenHeight);
            return;
        }

        var height = _levelHeightInPixels - ViewPortY;
        var screenStart = height * scaleMultiplier;
        VerticalRenderIntervals[0] = new RenderInterval(ViewPortY, height, 0, screenStart);

        var limit = _numberOfVerticalRenderIntervals - 1;

        for (var i = 1; i < limit; i++)
        {
            VerticalRenderIntervals[i] = new RenderInterval(0, _levelHeightInPixels, screenStart, ScreenHeight);
            screenStart += ScreenHeight;
        }

        var pixelLength = ViewPortHeight + ViewPortY - limit * _levelHeightInPixels;
        var screenHeight = pixelLength * scaleMultiplier;
        VerticalRenderIntervals[_numberOfVerticalRenderIntervals - 1] = new RenderInterval(0, pixelLength, screenStart, screenHeight);
    }
}