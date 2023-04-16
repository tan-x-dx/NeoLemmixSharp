using System.Collections.Generic;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalVoidViewPortBehaviour : IVerticalViewPortBehaviour
{
    private readonly SimpleList _verticalRenderIntervals;

    public int LevelHeightInPixels { get; }
    public int ViewPortY { get; private set; }
    public int ViewPortHeight { get; private set; }
    public int ScreenY { get; private set; }
    public int ScreenHeight { get; private set; }

    public IReadOnlyList<RenderInterval> VerticalRenderIntervals => _verticalRenderIntervals;

    public VerticalVoidViewPortBehaviour(int levelHeightInPixels)
    {
        LevelHeightInPixels = levelHeightInPixels;

        _verticalRenderIntervals = new SimpleList(1, 1);
    }

    public int NormaliseY(int y)
    {
        return y;
    }

    public void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight)
    {
        ViewPortHeight = (windowHeight - 64) / scaleMultiplier;

        if (ViewPortHeight < LevelHeightInPixels)
        {
            ScreenY = 0;
            ScreenHeight = ViewPortHeight * scaleMultiplier;
        }
        else
        {
            ScreenY = scaleMultiplier * (ViewPortHeight - LevelHeightInPixels) / 2;
            ScreenHeight = LevelHeightInPixels * scaleMultiplier;
            ViewPortHeight = LevelHeightInPixels;
        }
    }

    public void ScrollVertically(int dy)
    {
        if (ViewPortHeight >= LevelHeightInPixels)
        {
            ViewPortY = 0;
        }
        else
        {
            ViewPortY += dy;
            if (ViewPortY < 0)
            {
                ViewPortY = 0;
            }
            else if (ViewPortY + ViewPortHeight >= LevelHeightInPixels)
            {
                ViewPortY = LevelHeightInPixels - ViewPortHeight;
            }
        }
    }

    public void RecalculateVerticalRenderIntervals(int scaleMultiplier)
    {
        VerticalRenderIntervals[0].PixelStart = ViewPortY;
        VerticalRenderIntervals[0].PixelLength = ViewPortHeight;
        VerticalRenderIntervals[0].ScreenStart = ScreenY;
        VerticalRenderIntervals[0].ScreenLength = ScreenHeight;
    }
}