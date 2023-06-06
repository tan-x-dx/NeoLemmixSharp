using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalVoidBoundaryBehaviour : IHorizontalBoundaryBehaviour
{
    public int NormaliseX(int x) => x;
    public int GetAbsoluteHorizontalDistance(int x1, int x2)
    {
        return Math.Abs(x1 - x2);
    }
}