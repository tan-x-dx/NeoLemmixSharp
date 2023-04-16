using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalWrapViewPortBehaviour : IVerticalViewPortBehaviour
{
    private readonly SimpleList _verticalRenderIntervals;

    private int _numberOfVerticalRenderIntervals = 1;

    public int LevelHeightInPixels { get; }
    public int ViewPortY { get; private set; }
    public int ViewPortHeight { get; private set; }
    public int ScreenY => 0;
    public int ScreenHeight { get; private set; }

    public IReadOnlyList<RenderInterval> VerticalRenderIntervals => _verticalRenderIntervals;

    public VerticalWrapViewPortBehaviour(int levelHeightInPixels)
    {
        LevelHeightInPixels = levelHeightInPixels;

        _verticalRenderIntervals = new SimpleList(5, 1);
    }

    public int NormaliseY(int y)
    {
        if (y < 0)
            return y + LevelHeightInPixels;

        if (y < LevelHeightInPixels)
            return y;

        y -= LevelHeightInPixels;

        if (y >= LevelHeightInPixels)
            return y % LevelHeightInPixels;

        return y;
    }

    public void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight)
    {
        ViewPortHeight = (windowHeight - 64) / scaleMultiplier;

        if (ViewPortHeight < LevelHeightInPixels)
        {
            ScreenHeight = ViewPortHeight * scaleMultiplier;
        }
        else
        {
            ScreenHeight = LevelHeightInPixels * scaleMultiplier;
        }
    }

    public void ScrollVertically(int dy)
    {
        ViewPortY += dy;
        if (ViewPortY < 0)
        {
            ViewPortY += LevelHeightInPixels;
        }
        else if (ViewPortY >= LevelHeightInPixels)
        {
            ViewPortY -= LevelHeightInPixels;
        }
    }

    public void RecalculateVerticalRenderIntervals(int scaleMultiplier)
    {
        var previousNumberOfVerticalRenderIntervals = _numberOfVerticalRenderIntervals;
        _numberOfVerticalRenderIntervals = Math.Clamp(1 + (ViewPortY + ViewPortHeight - 1) / LevelHeightInPixels, 1, 5);

        if (_numberOfVerticalRenderIntervals != previousNumberOfVerticalRenderIntervals)
        {
            _verticalRenderIntervals.SetSize(_numberOfVerticalRenderIntervals);
        }

        if (_numberOfVerticalRenderIntervals == 1)
        {
            _verticalRenderIntervals.SetData(0, ViewPortY, ViewPortHeight, ScreenY, ScreenHeight);
            return;
        }

        var height = LevelHeightInPixels - ViewPortY;
        var screenStart = height * scaleMultiplier;
        _verticalRenderIntervals.SetData(0, ViewPortY, height, 0, screenStart);

        var limit = _numberOfVerticalRenderIntervals - 1;

        for (var i = 1; i < limit; i++)
        {
            _verticalRenderIntervals.SetData(i, 0, LevelHeightInPixels, screenStart, ScreenHeight);
            screenStart += ScreenHeight;
        }

        var pixelLength = ViewPortHeight + ViewPortY - limit * LevelHeightInPixels;
        var screenHeight = pixelLength * scaleMultiplier;
        _verticalRenderIntervals.SetData(_numberOfVerticalRenderIntervals - 1, 0, pixelLength, screenStart, screenHeight);
    }
}