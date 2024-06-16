namespace NeoLemmixSharp.Common.BoundaryBehaviours;

public readonly struct ScreenRenderInterval
{
    public readonly int ViewPortCoordinate;
    public readonly int ViewPortDimension;
    public readonly int ScreenCoordinate;
    public readonly int ScreenDimension;

    public ScreenRenderInterval(int viewPortCoordinate, int viewPortDimension, int screenCoordinate, int screenDimension)
    {
        ViewPortCoordinate = viewPortCoordinate;
        ViewPortDimension = viewPortDimension;
        ScreenCoordinate = screenCoordinate;
        ScreenDimension = screenDimension;
    }
}