namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public interface IVerticalBoundaryBehaviour
{
    int NormaliseY(int y);
    int GetVerticalDistanceSquared(int y1, int y2);
}