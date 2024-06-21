using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("C: {ViewPortCoordinate}, D: {ViewPortDimension}, O: {Offset}, S: {SpriteBoundaryShift}")]
public readonly struct ViewPortRenderInterval
{
    public readonly int ViewPortCoordinate;
    public readonly int ViewPortDimension;
    public readonly int Offset;
    public readonly int SpriteBoundaryShift;

    public ViewPortRenderInterval(int viewPortCoordinate, int viewPortDimension, int offset, int spriteBoundaryShift)
    {
        ViewPortCoordinate = viewPortCoordinate;
        ViewPortDimension = viewPortDimension;
        Offset = offset;
        SpriteBoundaryShift = spriteBoundaryShift;
    }
}