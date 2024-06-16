using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

public sealed class VerticalVoidBoundaryBehaviour : IVerticalBoundaryBehaviourAaa
{
    public BoundaryBehaviourType BoundaryBehaviourType => BoundaryBehaviourType.Void;
    public int LevelHeight { get; }

    public VerticalVoidBoundaryBehaviour(int levelHeight)
    {
        LevelHeight = levelHeight;
    }

    [Pure]
    public int NormaliseY(int y) => y;

    public void NormaliseYCoords(ref int top, ref int bottom, ref int y)
    {
        // Do nothing - coords will already be fine
    }

    [Pure]
    public int GetVerticalDelta(int y1, int y2) => y2 - y1;
}