using System.Diagnostics;

namespace NeoLemmixSharp.Common.Util.LevelRegion;

public sealed class EmptyLevelRegion : ILevelRegion
{
    public static EmptyLevelRegion Instance { get; } = new();

    private EmptyLevelRegion()
    {
    }

    public bool ContainsPoint(LevelPosition levelPosition) => false;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsEmpty => true;
}