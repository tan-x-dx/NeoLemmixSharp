using System.Collections.Generic;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidViewPortBehaviour : IHorizontalViewPortBehaviour
{
    private readonly SimpleList _horizontalRenderIntervals;

    public int LevelWidthInPixels { get; }
    public int ViewPortX { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ScreenX { get; private set; }
    public int ScreenWidth { get; private set; }

    public IReadOnlyList<RenderInterval> HorizontalRenderIntervals => _horizontalRenderIntervals;

    public HorizontalVoidViewPortBehaviour(int levelWidthInPixels)
    {
        LevelWidthInPixels = levelWidthInPixels;

        _horizontalRenderIntervals = new SimpleList(1, 1);
    }

    public int NormaliseX(int x)
    {
        return x;
    }

    public void RecalculateHorizontalDimensions(int scaleMultiplier, int windowWidth)
    {
        ViewPortWidth = windowWidth / scaleMultiplier;

        if (ViewPortWidth < LevelWidthInPixels)
        {
            ScreenX = 0;
            ScreenWidth = ViewPortWidth * scaleMultiplier;
        }
        else
        {
            ScreenX = scaleMultiplier * (ViewPortWidth - LevelWidthInPixels) / 2;
            ScreenWidth = LevelWidthInPixels * scaleMultiplier;
            ViewPortWidth = LevelWidthInPixels;
        }
    }

    public void ScrollHorizontally(int dx)
    {
        if (ViewPortWidth >= LevelWidthInPixels)
        {
            ViewPortX = 0;
        }
        else
        {
            ViewPortX += dx;
            if (ViewPortX < 0)
            {
                ViewPortX = 0;
            }
            else if (ViewPortX + ViewPortWidth >= LevelWidthInPixels)
            {
                ViewPortX = LevelWidthInPixels - ViewPortWidth;
            }
        }
    }

    public void RecalculateHorizontalRenderIntervals(int scaleMultiplier)
    {
        _horizontalRenderIntervals.SetData(0, ViewPortX, ViewPortWidth, ScreenX, ScreenWidth);
    }
}