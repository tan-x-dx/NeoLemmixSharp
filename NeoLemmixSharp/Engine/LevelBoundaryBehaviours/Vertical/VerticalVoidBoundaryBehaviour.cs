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

    public void ScrollViewPortVertically(LevelViewPort viewPort, int dy)
    {
        throw new System.NotImplementedException();
    }

    public int NormaliseY(int y)
    {
        return y;
    }
}