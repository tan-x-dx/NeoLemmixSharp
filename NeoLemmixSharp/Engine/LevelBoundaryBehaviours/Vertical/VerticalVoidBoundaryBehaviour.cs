namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalVoidBoundaryBehaviour : IVerticalBoundaryBehaviour
{
    public int NormaliseY(int y) => y;

    public int GetVerticalDistanceSquared(int y1, int y2)
    {
        var dy = y1 - y2;
        return dy * dy;
    }
}