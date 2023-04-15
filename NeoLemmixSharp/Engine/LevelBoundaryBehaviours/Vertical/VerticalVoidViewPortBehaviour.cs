namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalVoidViewPortBehaviour : IVerticalViewPortBehaviour
{
    private readonly int _height;

    public int ViewPortY { get; private set; }
    public int ViewPortHeight { get; private set; }
    public int ScreenY { get; private set; }
    public int ScreenHeight { get; private set; }
    public int NumberOfVerticalTilings => 1;

    public VerticalVoidViewPortBehaviour(int height)
    {
        _height = height;
    }

    public int NormaliseY(int y)
    {
        return y;
    }

    public void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight)
    {
        ViewPortHeight = (windowHeight - 64) / scaleMultiplier;

        if (ViewPortHeight < _height)
        {
            ScreenY = 0;
            ScreenHeight = ViewPortHeight * scaleMultiplier;
        }
        else
        {
            ScreenY = scaleMultiplier * (ViewPortHeight - _height) / 2;
            ScreenHeight = _height * scaleMultiplier;
            ViewPortHeight = _height;
        }

        ScrollVertically(0);
    }

    public void ScrollVertically(int dy)
    {
        if (ViewPortHeight >= _height)
        {
            ViewPortY = 0;
            return;
        }

        ViewPortY += dy;
        if (ViewPortY < 0)
        {
            ViewPortY = 0;
        }
        else if (ViewPortY + ViewPortHeight >= _height)
        {
            ViewPortY = _height - ViewPortHeight;
        }
    }
}