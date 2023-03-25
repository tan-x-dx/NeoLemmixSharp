namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public interface ILevelBoundaryBehaviour
{
    LevelPosition NormalisePosition(LevelPosition levelPosition);

    void ScrollViewPortHorizontally(LevelViewPort viewPort, int dx);
    void ScrollViewPortVertically(LevelViewPort viewPort, int dy);
}