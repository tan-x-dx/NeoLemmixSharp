namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public sealed class HorizontalWrapBehaviour : IHorizontalViewPortBehaviour
{
    private readonly RenderInterval[] _horizontalRenderIntervals;

    public int LevelWidthInPixels { get; }
    public int ViewPortX { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ScreenX => 0;
    public int ScreenWidth { get; private set; }
    public int NumberOfHorizontalRenderIntervals { get; private set; } = 1;

    public HorizontalWrapBehaviour(int levelWidthInPixels)
    {
        LevelWidthInPixels = levelWidthInPixels;

        _horizontalRenderIntervals = new RenderInterval[5];
        for (var i = 0; i < _horizontalRenderIntervals.Length; i++)
        {
            _horizontalRenderIntervals[i] = new RenderInterval();
        }
    }

    public RenderInterval GetHorizontalRenderInterval(int i) => _horizontalRenderIntervals[i];

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
        NumberOfHorizontalRenderIntervals = Math.Clamp(1 + (ViewPortX + ViewPortWidth - 1) / LevelWidthInPixels, 1, 5);

        if (NumberOfHorizontalRenderIntervals == 1)
        {
            SetRenderIntervalData(0, ViewPortX, ViewPortWidth, ScreenX, ScreenWidth);

            return;
        }

        var width = LevelWidthInPixels - ViewPortX;
        var screenStart = width * scaleMultiplier;
        SetRenderIntervalData(0, ViewPortX, width, 0, screenStart);

        var limit = NumberOfHorizontalRenderIntervals - 1;

        for (var i = 1; i < limit; i++)
        {
            SetRenderIntervalData(i, 0, LevelWidthInPixels, screenStart, ScreenWidth);
            screenStart += ScreenWidth;
        }

        var pixelLength = ViewPortWidth + ViewPortX - limit * LevelWidthInPixels;
        var screenWidth = pixelLength * scaleMultiplier;
        SetRenderIntervalData(NumberOfHorizontalRenderIntervals - 1, 0, pixelLength, screenStart, screenWidth);
    }

    private void SetRenderIntervalData(int index, int pixelStart, int pixelLength, int screenStart, int screenLength)
    {
        var item = _horizontalRenderIntervals[index];
        item.PixelStart = pixelStart;
        item.PixelLength = pixelLength;
        item.ScreenStart = screenStart;
        item.ScreenLength = screenLength;
    }
}