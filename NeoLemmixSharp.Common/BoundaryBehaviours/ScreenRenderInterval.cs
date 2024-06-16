using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("vpC: {ViewPortCoordinate}, vpD: {ViewPortDimension}, sC: {ScreenCoordinate}, sD: {ScreenDimension}")]
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