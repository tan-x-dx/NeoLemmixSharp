using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

public sealed class VerticalWrapBoundaryBehaviour : IVerticalBoundaryBehaviour
{
    public BoundaryBehaviourType BoundaryBehaviourType => BoundaryBehaviourType.Wrap;
    public int LevelHeight { get; }

    public VerticalWrapBoundaryBehaviour(int levelHeightInPixels)
    {
        LevelHeight = levelHeightInPixels;
    }

    [Pure]
    public int NormaliseY(int y)
    {
        // most likely case for negatives will be "small" numbers. Therefore simply adding the level height will make it a valid value
        if (y < 0)
            return y + LevelHeight;

        if (y < LevelHeight)
            return y;

        // most likely case for "big" numbers will be less than twice the level height. Therefore simply subtracting the level height will make it a valid value
        y -= LevelHeight;

        if (y < LevelHeight)
            return y;

        // otherwise, just do modulo operation
        return y % LevelHeight;
    }

    public void NormaliseYCoords(ref int top, ref int bottom, ref int y)
    {
        if (bottom < LevelHeight)
            return;

        var halfLevelHeight = LevelHeight / 2;
        top -= halfLevelHeight;
        bottom -= halfLevelHeight;
        y -= halfLevelHeight;
    }

    [Pure]
    public int GetVerticalDelta(int y1, int y2)
    {
        var dy = y2 - y1;

        if (dy > 0)
        {
            if (dy + dy > LevelHeight)
                return dy - LevelHeight;

            return dy;
        }

        if (dy + dy < -LevelHeight)
            return dy + LevelHeight;

        return dy;
    }
}