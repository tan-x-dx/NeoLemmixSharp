using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalVoidBoundaryBehaviour : IVerticalBoundaryBehaviour
{
    public int NormaliseY(int y) => y;
    public int GetAbsoluteVerticalDistance(int y1, int y2) => Math.Abs(y1 - y2);
}