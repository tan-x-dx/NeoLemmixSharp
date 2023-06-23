using System;

namespace NeoLemmixSharp.Engine.BoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidBoundaryBehaviour : IHorizontalBoundaryBehaviour
{
    public int NormaliseX(int x) => x;
    public int GetAbsoluteHorizontalDistance(int x1, int x2) => Math.Abs(x1 - x2);
}