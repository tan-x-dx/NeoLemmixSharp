namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidBoundaryBehaviour : IHorizontalBoundaryBehaviour
{
    public int NormaliseX(int x) => x;

    public int GetHorizontalDistanceSquared(int x1, int x2)
    {
        var dx = x1 - x2;
        return dx * dx;
    }
}