using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

public interface IVerticalBoundaryBehaviour
{
    BoundaryBehaviourType BoundaryBehaviourType { get; }
    int LevelHeight { get; }

    [Pure]
    int NormaliseY(int y);

    void NormaliseYCoords(ref int top, ref int bottom, ref int y);

    [Pure]
    int GetVerticalDelta(int y1, int y2);
}