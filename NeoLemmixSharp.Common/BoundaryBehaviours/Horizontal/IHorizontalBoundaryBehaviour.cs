using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public interface IHorizontalBoundaryBehaviour
{
    int LevelWidth { get; }

    [Pure]
    int NormaliseX(int x);
    [Pure]
    int GetAbsoluteHorizontalDistance(int x1, int x2);
}