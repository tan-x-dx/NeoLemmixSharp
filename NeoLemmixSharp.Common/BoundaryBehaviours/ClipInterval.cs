using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("S: {Start}, L: {Length}, O: {Offset}")]
[method: DebuggerStepThrough]
public readonly ref struct ClipInterval(int start, int length, int offset)
{
    public readonly int Start = start;
    public readonly int Length = length;
    public readonly int Offset = offset;
}