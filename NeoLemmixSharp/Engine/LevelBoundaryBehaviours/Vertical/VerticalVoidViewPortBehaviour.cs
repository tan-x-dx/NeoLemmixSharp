namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalVoidViewPortBehaviour : IVerticalViewPortBehaviour
{
    private readonly int _levelHeightInPixels;

    public int ViewPortY { get; private set; }
    public int ViewPortHeight { get; private set; }
    public int ScreenY { get; private set; }
    public int ScreenHeight { get; private set; }

    public RenderInterval[] VerticalRenderIntervals { get; }

    public VerticalVoidViewPortBehaviour(int levelHeightInPixels)
    {
        _levelHeightInPixels = levelHeightInPixels;
        VerticalRenderIntervals = new[]
        {
            new RenderInterval(ViewPortY, ViewPortHeight, ScreenY, ScreenHeight)
        };
    }

    public int NormaliseY(int y)
    {
        return y;
    }

    public void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight)
    {
        ViewPortHeight = (windowHeight - 64) / scaleMultiplier;

        if (ViewPortHeight < _levelHeightInPixels)
        {
            ScreenY = 0;
            ScreenHeight = ViewPortHeight * scaleMultiplier;
        }
        else
        {
            ScreenY = scaleMultiplier * (ViewPortHeight - _levelHeightInPixels) / 2;
            ScreenHeight = _levelHeightInPixels * scaleMultiplier;
            ViewPortHeight = _levelHeightInPixels;
        }
    }

    public void ScrollVertically(int dy)
    {
        if (ViewPortHeight >= _levelHeightInPixels)
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
            else if (ViewPortY + ViewPortHeight >= _levelHeightInPixels)
            {
                ViewPortY = _levelHeightInPixels - ViewPortHeight;
            }
        }
    }

    public void RecalculateVerticalRenderIntervals(int scaleMultiplier)
    {
        VerticalRenderIntervals[0] = new RenderInterval(ViewPortY, ViewPortHeight, ScreenY, ScreenHeight);
    }
}