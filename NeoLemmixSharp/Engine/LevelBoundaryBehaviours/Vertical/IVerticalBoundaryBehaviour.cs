namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public interface IVerticalBoundaryBehaviour
{
    int NormaliseY(int y);
    void ScrollViewPortVertically(LevelViewPort viewPort, int dy);
}
