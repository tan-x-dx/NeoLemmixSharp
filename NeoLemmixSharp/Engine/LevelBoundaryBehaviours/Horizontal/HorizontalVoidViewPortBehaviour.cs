namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidViewPortBehaviour : IHorizontalViewPortBehaviour
{
    private readonly RenderInterval _renderInterval;

    public int LevelWidthInPixels { get; }
    public int ViewPortX { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ScreenX { get; private set; }
    public int ScreenWidth { get; private set; }
    public int NumberOfHorizontalRenderIntervals => 1;

    public HorizontalVoidViewPortBehaviour(int levelWidthInPixels)
    {
        LevelWidthInPixels = levelWidthInPixels;

        _renderInterval = new RenderInterval();
    }

    public RenderInterval GetHorizontalRenderInterval(int i) => _renderInterval;

    public int NormaliseX(int x) => x;

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
        _renderInterval.PixelStart = ViewPortX;
        _renderInterval.PixelLength = ViewPortWidth;
        _renderInterval.ScreenStart = ScreenX;
        _renderInterval.ScreenLength = ScreenWidth;
    }
}