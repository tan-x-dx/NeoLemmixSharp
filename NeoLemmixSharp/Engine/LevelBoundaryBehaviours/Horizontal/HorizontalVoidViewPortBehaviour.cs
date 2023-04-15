namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidViewPortBehaviour : IHorizontalViewPortBehaviour
{
    private readonly int _width;

    public int ViewPortX { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ScreenX { get; private set; }
    public int ScreenWidth { get; private set; }
    public int NumberOfHorizontalTilings => 1;

    public HorizontalVoidViewPortBehaviour(int width)
    {
        _width = width;
    }

    public int NormaliseX(int x)
    {
        return x;
    }

    public void RecalculateHorizontalDimensions(int scaleMultiplier, int windowWidth)
    {
        ViewPortWidth = windowWidth / scaleMultiplier;

        if (ViewPortWidth < _width)
        {
            ScreenX = 0;
            ScreenWidth = ViewPortWidth * scaleMultiplier;
        }
        else
        {
            ScreenX = scaleMultiplier * (ViewPortWidth - _width) / 2;
            ScreenWidth = _width * scaleMultiplier;
            ViewPortWidth = _width;
        }

        ScrollHorizontally(0);
    }

    public void ScrollHorizontally(int dx)
    {
        if (ViewPortWidth >= _width)
        {
            ViewPortX = 0;
            return;
        }

        ViewPortX += dx;
        if (ViewPortX < 0)
        {
            ViewPortX = 0;
        }
        else if (ViewPortX + ViewPortWidth >= _width)
        {
            ViewPortX = _width - ViewPortWidth;
        }
    }
}