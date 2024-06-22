using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("C: {ViewPortCoordinate}, D: {ViewPortDimension}, O: {Offset}")]
public readonly struct ViewPortRenderInterval
{
    public readonly int ViewPortCoordinate;
    public readonly int ViewPortDimension;
    public readonly int Offset;

    [DebuggerStepThrough]
    public ViewPortRenderInterval(int viewPortCoordinate, int viewPortDimension, int offset)
    {
        ViewPortCoordinate = viewPortCoordinate;
        ViewPortDimension = viewPortDimension;
        Offset = offset;
    }
}