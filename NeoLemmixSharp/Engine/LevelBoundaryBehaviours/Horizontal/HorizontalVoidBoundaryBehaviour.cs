namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidBoundaryBehaviour : IHorizontalBoundaryBehaviour
{
    private readonly int _width;
    private readonly int _height;

    public HorizontalVoidBoundaryBehaviour(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public int NormaliseX(int x)
    {
        return x;
    }

    public void ScrollViewPortHorizontally(LevelViewPort viewPort, int dx)
    {
        if (viewPort.ViewPortWidth >= _width)
        {
            viewPort.ViewPortX = 0;
            return;
        }

        viewPort.ViewPortX += dx;
        if (viewPort.ViewPortX < 0)
        {
            viewPort.ViewPortX = 0;
        }
        else if (viewPort.ViewPortX + viewPort.ViewPortWidth >= _width)
        {
            viewPort.ViewPortX = _width - viewPort.ViewPortWidth;
        }
    }
}