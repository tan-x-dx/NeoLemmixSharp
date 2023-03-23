namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public interface ILevelBoundaryBehaviour
{
    PixelData GetPixel(ref LevelPosition levelPosition);

    void ScrollViewPortHorizontally(LevelViewPort viewPort, int dx);
    void ScrollViewPortVertically(LevelViewPort viewPort, int dy);
}