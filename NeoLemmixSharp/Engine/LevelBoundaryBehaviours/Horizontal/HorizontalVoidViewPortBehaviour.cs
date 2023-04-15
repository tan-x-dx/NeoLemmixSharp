namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidViewPortBehaviour : IHorizontalViewPortBehaviour
{
    private readonly int _levelWidthInPixels;

    public int ViewPortX { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ScreenX { get; private set; }
    public int ScreenWidth { get; private set; }

    public RenderInterval[] HorizontalRenderIntervals { get; }

    public HorizontalVoidViewPortBehaviour(int levelWidthInPixels)
    {
        _levelWidthInPixels = levelWidthInPixels;

        HorizontalRenderIntervals = new[]
        {
            new RenderInterval(ViewPortX, ViewPortWidth, ScreenX, ScreenWidth)
        };
    }

    public int NormaliseX(int x)
    {
        return x;
    }

    public void RecalculateHorizontalDimensions(int scaleMultiplier, int windowWidth)
    {
        ViewPortWidth = windowWidth / scaleMultiplier;

        if (ViewPortWidth < _levelWidthInPixels)
        {
            ScreenX = 0;
            ScreenWidth = ViewPortWidth * scaleMultiplier;
        }
        else
        {
            ScreenX = scaleMultiplier * (ViewPortWidth - _levelWidthInPixels) / 2;
            ScreenWidth = _levelWidthInPixels * scaleMultiplier;
            ViewPortWidth = _levelWidthInPixels;
        }
    }

    public void ScrollHorizontally(int dx)
    {
        if (ViewPortWidth >= _levelWidthInPixels)
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
            else if (ViewPortX + ViewPortWidth >= _levelWidthInPixels)
            {
                ViewPortX = _levelWidthInPixels - ViewPortWidth;
            }
        }
    }

    public void RecalculateHorizontalRenderIntervals(int scaleMultiplier)
    {
        HorizontalRenderIntervals[0] = new RenderInterval(ViewPortX, ViewPortWidth, ScreenX, ScreenWidth);
    }
}