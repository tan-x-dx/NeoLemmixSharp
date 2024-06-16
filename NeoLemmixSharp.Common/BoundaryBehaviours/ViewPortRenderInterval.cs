using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("pixelStart: {ViewPortCoordinate}, pixelLength: {ViewPortDimension}, pos: {Offset}")]
public readonly struct ViewPortRenderInterval
{
    public readonly int ViewPortCoordinate;
    public readonly int ViewPortDimension;
    public readonly int Offset;

    public ViewPortRenderInterval(int viewPortCoordinate, int viewPortDimension, int offset)
    {
        ViewPortCoordinate = viewPortCoordinate;
        ViewPortDimension = viewPortDimension;
        Offset = offset;
    }
}