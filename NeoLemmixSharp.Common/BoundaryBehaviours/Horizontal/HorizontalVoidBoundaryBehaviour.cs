using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidBoundaryBehaviour : IHorizontalBoundaryBehaviour
{
    public int LevelWidth { get; }

    public HorizontalVoidBoundaryBehaviour(int levelWidth)
    {
        LevelWidth = levelWidth;
    }

    [Pure]
    public int NormaliseX(int x) => x;
    [Pure]
    public int GetAbsoluteHorizontalDistance(int x1, int x2) => Math.Abs(x1 - x2);
}