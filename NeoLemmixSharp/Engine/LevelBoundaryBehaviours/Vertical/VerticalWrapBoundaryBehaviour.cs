namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalWrapBoundaryBehaviour : IVerticalBoundaryBehaviour
{
    private readonly int _width;
    private readonly int _height;

    public VerticalWrapBoundaryBehaviour(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public int NormaliseY(int y)
    {
        if (y < 0)
            return y + _height;

        if (y >= _height)
            return y - _height;

        return y;
    }

    public void ScrollViewPortVertically(LevelViewPort viewPort, int dy)
    {
        viewPort.ViewPortY += dy;
        if (viewPort.ViewPortY < 0)
        {
            viewPort.ViewPortY += _height;
        }
        else if (viewPort.ViewPortY >= _height)
        {
            viewPort.ViewPortY -= _height;
        }
    }
}