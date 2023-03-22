namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public sealed class VoidBoundaryBehaviour : ILevelBoundaryBehaviour
{
    public PixelData GetPixel(int x, int y)
    {
        return null;
    }
}