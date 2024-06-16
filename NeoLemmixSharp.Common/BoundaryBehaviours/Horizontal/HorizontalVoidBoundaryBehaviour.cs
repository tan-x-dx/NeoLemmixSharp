using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidBoundaryBehaviour : IHorizontalBoundaryBehaviourAaa
{
    public BoundaryBehaviourType BoundaryBehaviourType => BoundaryBehaviourType.Void;
    public int LevelWidth { get; }

    public HorizontalVoidBoundaryBehaviour(int levelWidth)
    {
        LevelWidth = levelWidth;
    }

    [Pure]
    public int NormaliseX(int x) => x;

    public void NormaliseXCoords(ref int left, ref int right, ref int x)
    {
        // Do nothing - coords will already be fine
    }

    [Pure]
    public int GetHorizontalDelta(int x1, int x2) => x2 - x1;
}