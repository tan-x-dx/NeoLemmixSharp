namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public interface IHorizontalBoundaryBehaviour
{
    int NormaliseX(int x);
    int GetHorizontalDistanceSquared(int x1, int x2);
}