using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

public interface IVerticalBoundaryBehaviour
{
    int LevelHeight { get; }

    [Pure]
    int NormaliseY(int y);
    [Pure]
    int GetAbsoluteVerticalDistance(int y1, int y2);
}