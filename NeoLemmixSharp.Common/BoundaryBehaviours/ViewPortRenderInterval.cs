using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("S: {ViewPortStart}, L: {ViewPortLength}, O: {Offset}")]
[method: DebuggerStepThrough]
public readonly struct ViewPortRenderInterval(int viewPortStart, int viewPortLength, int offset)
{
    public readonly int ViewPortStart = viewPortStart;
    public readonly int ViewPortLength = viewPortLength;
    public readonly int Offset = offset;
}
