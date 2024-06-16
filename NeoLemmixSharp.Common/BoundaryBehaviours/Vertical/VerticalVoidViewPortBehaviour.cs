namespace NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

public sealed class VerticalVoidViewPortBehaviour : IVerticalViewPortBehaviourAaa
{
    private readonly RenderInterval _renderInterval;

    public int LevelHeightInPixels { get; }
    public int ViewPortY { get; private set; }
    public int ViewPortHeight { get; private set; }
    public int ScreenY { get; private set; }
    public int ScreenHeight { get; private set; }
    public int NumberOfVerticalRenderIntervals => 1;

    public VerticalVoidViewPortBehaviour(int levelHeightInPixels)
    {
        LevelHeightInPixels = levelHeightInPixels;
        _renderInterval = new RenderInterval();
    }

    public RenderInterval GetVerticalRenderInterval(int i) => _renderInterval;

    public void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight, int controlPanelHeight)
    {
        ViewPortHeight = (windowHeight + scaleMultiplier - controlPanelHeight) / scaleMultiplier;

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
      /*  _renderInterval.PixelStart = ViewPortY;
        _renderInterval.PixelLength = ViewPortHeight;
        _renderInterval.ScreenStart = 0;*/
    }

    public ReadOnlySpan<RenderInterval> GetVerticalRenderIntervals(Span<RenderInterval> baseSpan)
    {
        baseSpan[0] = new RenderInterval(
            _renderInterval.PixelStart,
            _renderInterval.PixelLength);

        return baseSpan[..1];
    }
}