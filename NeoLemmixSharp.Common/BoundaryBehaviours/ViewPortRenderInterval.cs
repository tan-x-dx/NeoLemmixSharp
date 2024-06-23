using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("S: {ViewPortStart}, L: {ViewPortLength}, O: {Offset}")]
public readonly struct ViewPortRenderInterval
{
    public readonly int ViewPortStart;
    public readonly int ViewPortLength;
    public readonly int Offset;

    [DebuggerStepThrough]
    public ViewPortRenderInterval(int viewPortStart, int viewPortLength, int offset)
    {
        ViewPortStart = viewPortStart;
        ViewPortLength = viewPortLength;
        Offset = offset;
    }
}