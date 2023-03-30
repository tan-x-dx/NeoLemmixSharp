namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public interface IHorizontalBoundaryBehaviour
{
    int NormaliseX(int x);
    void ScrollViewPortHorizontally(LevelViewPort viewPort, int dx);
}