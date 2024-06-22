using System.Diagnostics;

namespace NeoLemmixSharp.Common.BoundaryBehaviours;

[DebuggerDisplay("S: {Start}, L: {Length}, O: {Offset}")]
public readonly ref struct ClipInterval
{
    public readonly int Start;
    public readonly int Length;
    public readonly int Offset;

    [DebuggerStepThrough]
    public ClipInterval(int start, int length, int offset)
    {
        Start = start;
        Length = length;
        Offset = offset;
    }
}