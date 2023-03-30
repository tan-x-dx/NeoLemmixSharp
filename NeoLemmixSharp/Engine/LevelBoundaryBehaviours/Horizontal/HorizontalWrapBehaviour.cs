namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalWrapBehaviour : IHorizontalBoundaryBehaviour
{
    private readonly int _width;
    private readonly int _height;

    public HorizontalWrapBehaviour(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public int NormaliseX(int x)
    {
        if (x < 0)
            return x + _width;

        if (x >= _width)
            return x - _width;

        return x;
    }

    public void ScrollViewPortHorizontally(LevelViewPort viewPort, int dx)
    {
        throw new System.NotImplementedException();
    }
}