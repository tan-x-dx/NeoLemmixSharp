namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public interface IVerticalBoundaryBehaviour
{
    int NormaliseY(int y);
    int GetAbsoluteVerticalDistance(int y1, int y2);
}