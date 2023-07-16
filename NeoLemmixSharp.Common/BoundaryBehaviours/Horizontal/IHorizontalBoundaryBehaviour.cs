using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public interface IHorizontalBoundaryBehaviour
{
    [Pure]
    int NormaliseX(int x);
    [Pure]
    int GetAbsoluteHorizontalDistance(int x1, int x2);
}