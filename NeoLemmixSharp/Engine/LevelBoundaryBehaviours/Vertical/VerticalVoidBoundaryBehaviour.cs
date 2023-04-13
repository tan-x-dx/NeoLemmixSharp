namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalVoidBoundaryBehaviour : IVerticalBoundaryBehaviour
{
    private readonly int _width;
    private readonly int _height;

    public VerticalVoidBoundaryBehaviour(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public int NormaliseY(int y)
    {
        return y;
    }

    public void ScrollViewPortVertically(LevelViewPort viewPort, int dy)
    {
        if (viewPort.ViewPortHeight >= _height)
        {
            viewPort.ViewPortY = 0;
            return;
        }

        viewPort.ViewPortY += dy;
        if (viewPort.ViewPortY < 0)
        {
            viewPort.ViewPortY = 0;
        }
        else if (viewPort.ViewPortY + viewPort.ViewPortHeight >= _height)
        {
            viewPort.ViewPortY = _height - viewPort.ViewPortHeight;
        }
    }
}