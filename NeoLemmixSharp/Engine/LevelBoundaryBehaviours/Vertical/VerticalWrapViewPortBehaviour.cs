using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalWrapViewPortBehaviour : IVerticalViewPortBehaviour
{
    private readonly RenderInterval[] _verticalRenderIntervals;

    public int LevelHeightInPixels { get; }
    public int ViewPortY { get; private set; }
    public int ViewPortHeight { get; private set; }
    public int ScreenY => 0;
    public int ScreenHeight { get; private set; }
    public int NumberOfVerticalRenderIntervals { get; private set; } = 1;

    public VerticalWrapViewPortBehaviour(int levelHeightInPixels)
    {
        LevelHeightInPixels = levelHeightInPixels;

        _verticalRenderIntervals = new RenderInterval[5];
        for (var i = 0; i < _verticalRenderIntervals.Length; i++)
        {
            _verticalRenderIntervals[i] = new RenderInterval();
        }
    }

    public RenderInterval GetVerticalRenderInterval(int i) => _verticalRenderIntervals[i];

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

    public void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight, int controlPanelHeight)
    {
        ViewPortHeight = (scaleMultiplier + windowHeight - controlPanelHeight) / scaleMultiplier;

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
        NumberOfVerticalRenderIntervals = Math.Clamp(1 + (ViewPortY + ViewPortHeight - 1) / LevelHeightInPixels, 1, 5);

        if (NumberOfVerticalRenderIntervals == 1)
        {
            SetRenderIntervalData(0, ViewPortY, ViewPortHeight, ScreenY, ScreenHeight);
            return;
        }

        var height = LevelHeightInPixels - ViewPortY;
        var screenStart = height * scaleMultiplier;
        SetRenderIntervalData(0, ViewPortY, height, 0, screenStart);

        var limit = NumberOfVerticalRenderIntervals - 1;

        for (var i = 1; i < limit; i++)
        {
            SetRenderIntervalData(i, 0, LevelHeightInPixels, screenStart, ScreenHeight);
            screenStart += ScreenHeight;
        }

        var pixelLength = ViewPortHeight + ViewPortY - limit * LevelHeightInPixels;
        var screenHeight = pixelLength * scaleMultiplier;
        SetRenderIntervalData(NumberOfVerticalRenderIntervals - 1, 0, pixelLength, screenStart, screenHeight);
    }

    private void SetRenderIntervalData(int index, int pixelStart, int pixelLength, int screenStart, int screenLength)
    {
        var item = _verticalRenderIntervals[index];
        item.PixelStart = pixelStart;
        item.PixelLength = pixelLength;
        item.ScreenStart = screenStart;
        item.ScreenLength = screenLength;
    }
}