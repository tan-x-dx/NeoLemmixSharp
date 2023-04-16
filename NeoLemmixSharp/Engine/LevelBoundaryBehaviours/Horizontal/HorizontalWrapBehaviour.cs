using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalWrapBehaviour : IHorizontalViewPortBehaviour
{
    private readonly SimpleList _horizontalRenderIntervals;

    private int _numberOfHorizontalRenderIntervals = 1;

    public int LevelWidthInPixels { get; }
    public int ViewPortX { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ScreenX => 0;
    public int ScreenWidth { get; private set; }

    public IReadOnlyList<RenderInterval> HorizontalRenderIntervals => _horizontalRenderIntervals;

    public HorizontalWrapBehaviour(int levelWidthInPixels)
    {
        LevelWidthInPixels = levelWidthInPixels;

        _horizontalRenderIntervals = new SimpleList(5, 1);
    }

    public int NormaliseX(int x)
    {
        if (x < 0)
            return x + LevelWidthInPixels;

        if (x < LevelWidthInPixels)
            return x;

        x -= LevelWidthInPixels;

        if (x >= LevelWidthInPixels)
            return x % LevelWidthInPixels;

        return x;
    }

    public void RecalculateHorizontalDimensions(int scaleMultiplier, int windowWidth)
    {
        ViewPortWidth = windowWidth / scaleMultiplier;

        if (ViewPortWidth < LevelWidthInPixels)
        {
            ScreenWidth = ViewPortWidth * scaleMultiplier;
        }
        else
        {
            ScreenWidth = LevelWidthInPixels * scaleMultiplier;
        }
    }

    public void ScrollHorizontally(int dx)
    {
        ViewPortX += dx;
        if (ViewPortX < 0)
        {
            ViewPortX += LevelWidthInPixels;
        }
        else if (ViewPortX >= LevelWidthInPixels)
        {
            ViewPortX -= LevelWidthInPixels;
        }
    }

    public void RecalculateHorizontalRenderIntervals(int scaleMultiplier)
    {
        var previousNumberOfHorizontalRenderIntervals = _numberOfHorizontalRenderIntervals;
        _numberOfHorizontalRenderIntervals = Math.Clamp(1 + (ViewPortX + ViewPortWidth - 1) / LevelWidthInPixels, 1, 5);

        if (_numberOfHorizontalRenderIntervals != previousNumberOfHorizontalRenderIntervals)
        {
            _horizontalRenderIntervals.SetSize(_numberOfHorizontalRenderIntervals);
        }

        if (_numberOfHorizontalRenderIntervals == 1)
        {
            _horizontalRenderIntervals.SetData(0, ViewPortX, ViewPortWidth, ScreenX, ScreenWidth);

            return;
        }

        var width = LevelWidthInPixels - ViewPortX;
        var screenStart = width * scaleMultiplier;
        _horizontalRenderIntervals.SetData(0, ViewPortX, width, 0, screenStart);

        var limit = _numberOfHorizontalRenderIntervals - 1;

        for (var i = 1; i < limit; i++)
        {
            _horizontalRenderIntervals.SetData(i, 0, LevelWidthInPixels, screenStart, ScreenWidth);
            screenStart += ScreenWidth;
        }

        var pixelLength = ViewPortWidth + ViewPortX - limit * LevelWidthInPixels;
        var screenWidth = pixelLength * scaleMultiplier;
        _horizontalRenderIntervals.SetData(_numberOfHorizontalRenderIntervals - 1, 0, pixelLength, screenStart, screenWidth);
    }
}