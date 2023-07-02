namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public interface IHorizontalBoundaryBehaviour
{
    int NormaliseX(int x);
    int GetAbsoluteHorizontalDistance(int x1, int x2);
}