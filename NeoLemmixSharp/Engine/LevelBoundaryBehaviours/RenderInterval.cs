namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public readonly struct RenderInterval
{
    public readonly int PixelStart;
    public readonly int PixelLength;

    public readonly int ScreenStart;
    public readonly int ScreenLength;

    public RenderInterval(int pixelStart, int pixelLength, int screenStart, int screenLength)
    {
        PixelStart = pixelStart;
        PixelLength = pixelLength;
        ScreenStart = screenStart;
        ScreenLength = screenLength;
    }
}