using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

public sealed class VerticalVoidBoundaryBehaviour : IVerticalBoundaryBehaviour
{
    public int LevelHeight { get; }

    public VerticalVoidBoundaryBehaviour(int levelHeight)
    {
        LevelHeight = levelHeight;
    }

    [Pure]
    public int NormaliseY(int y) => y;
    [Pure]
    public int GetAbsoluteVerticalDistance(int y1, int y2) => Math.Abs(y1 - y2);
}