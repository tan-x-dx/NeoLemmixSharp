using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalWrapBehaviour : IHorizontalViewPortBehaviour
{
    private readonly int _levelWidthInPixels;

    private int _numberOfHorizontalRenderIntervals = 1;

    public int ViewPortX { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ScreenX => 0;
    public int ScreenWidth { get; private set; }

    public RenderInterval[] HorizontalRenderIntervals { get; private set; }

    public HorizontalWrapBehaviour(int levelWidthInPixels)
    {
        _levelWidthInPixels = levelWidthInPixels;

        HorizontalRenderIntervals = new RenderInterval[1];
    }

    public int NormaliseX(int x)
    {
        if (x < 0)
            return x + _levelWidthInPixels;

        if (x >= _levelWidthInPixels)
            return x - _levelWidthInPixels;

        return x;
    }

    public void RecalculateHorizontalDimensions(int scaleMultiplier, int windowWidth)
    {
        ViewPortWidth = windowWidth / scaleMultiplier;

        if (ViewPortWidth < _levelWidthInPixels)
        {
            ScreenWidth = ViewPortWidth * scaleMultiplier;
        }
        else
        {
            ScreenWidth = _levelWidthInPixels * scaleMultiplier;
        }
    }

    public void ScrollHorizontally(int dx)
    {
        ViewPortX += dx;
        if (ViewPortX < 0)
        {
            ViewPortX += _levelWidthInPixels;
        }
        else if (ViewPortX >= _levelWidthInPixels)
        {
            ViewPortX -= _levelWidthInPixels;
        }
    }

    public void RecalculateHorizontalRenderIntervals(int scaleMultiplier)
    {
        var previousNumberOfHorizontalRenderIntervals = _numberOfHorizontalRenderIntervals;
        _numberOfHorizontalRenderIntervals = Math.Clamp(1 + (ViewPortX + ViewPortWidth - 1) / _levelWidthInPixels, 1, 5);

        if (_numberOfHorizontalRenderIntervals != previousNumberOfHorizontalRenderIntervals)
        {
            HorizontalRenderIntervals = new RenderInterval[_numberOfHorizontalRenderIntervals];
        }

        if (_numberOfHorizontalRenderIntervals == 1)
        {
            HorizontalRenderIntervals[0] = new RenderInterval(ViewPortX, ViewPortWidth, ScreenX, ScreenWidth);

            return;
        }

        var width = _levelWidthInPixels - ViewPortX;
        HorizontalRenderIntervals[0] = new RenderInterval(ViewPortX, width, 0, width * scaleMultiplier);

        var limit = _numberOfHorizontalRenderIntervals - 1;
        var screenStart = -(scaleMultiplier * ViewPortX);

        for (var i = 1; i < limit; i++)
        {
            screenStart += ScreenWidth;
            HorizontalRenderIntervals[i] = new RenderInterval(0, _levelWidthInPixels, screenStart, ScreenWidth);
        }

        var pixelLength = ViewPortWidth + ViewPortX - limit * _levelWidthInPixels;
        screenStart += ScreenWidth;
        var screenWidth = pixelLength * scaleMultiplier;
        HorizontalRenderIntervals[_numberOfHorizontalRenderIntervals - 1] = new RenderInterval(0, pixelLength, screenStart, screenWidth);
    }
}