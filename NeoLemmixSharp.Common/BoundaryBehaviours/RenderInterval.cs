namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public readonly struct RenderInterval
{
    public readonly int PixelStart;
    public readonly int PixelLength;

    public RenderInterval(int pixelStart, int pixelLength)
    {
        PixelStart = pixelStart;
        PixelLength = pixelLength;
    }
}