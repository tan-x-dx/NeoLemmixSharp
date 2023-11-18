using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public interface IHorizontalBoundaryBehaviour
{
    BoundaryBehaviourType BoundaryBehaviourType { get; }
    int LevelWidth { get; }

    [Pure]
    int NormaliseX(int x);
    
    void NormaliseXCoords(ref int left, ref int right, ref int x);

    [Pure]
    int GetHorizontalDelta(int x1, int x2);
}