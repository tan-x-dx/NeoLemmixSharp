namespace NeoLemmixSharp.Engine.BoundaryBehaviours;

public sealed class RenderInterval
{
    public int PixelStart;
    public int PixelLength;

    public int ScreenStart;
    public int ScreenLength;

    public bool Overlaps(int otherPixelStart, int otherPixelLength)
    {
        return PixelStart < otherPixelStart + otherPixelLength &&
               otherPixelStart < PixelStart + PixelLength;
    }
}