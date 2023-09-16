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
    public int GetHorizontalDelta(int x1, int x2) => x2 - x1;
}