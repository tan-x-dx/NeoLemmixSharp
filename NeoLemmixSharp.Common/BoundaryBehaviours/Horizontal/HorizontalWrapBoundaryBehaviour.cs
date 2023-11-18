using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public sealed class HorizontalWrapBoundaryBehaviour : IHorizontalBoundaryBehaviour
{
    public BoundaryBehaviourType BoundaryBehaviourType => BoundaryBehaviourType.Wrap;
    public int LevelWidth { get; }

    public HorizontalWrapBoundaryBehaviour(int levelWidthInPixels)
    {
        LevelWidth = levelWidthInPixels;
    }

    [Pure]
    public int NormaliseX(int x)
    {
        // most likely case for negatives will be "small" numbers. Therefore simply adding the level width will make it a valid value
        if (x < 0)
            return x + LevelWidth;

        if (x < LevelWidth)
            return x;

        // most likely case for "big" numbers will be less than twice the level width. Therefore simply subtracting the level width will make it a valid value
        x -= LevelWidth;

        if (x < LevelWidth)
            return x;

        // otherwise, just do modulo operation
        return x % LevelWidth;
    }

    public void NormaliseXCoords(ref int left, ref int right, ref int x)
    {
        if (right < LevelWidth)
            return;

        var halfLevelWidth = LevelWidth / 2;
        left -= halfLevelWidth;
        right -= halfLevelWidth;
        x -= halfLevelWidth;
    }

    [Pure]
    public int GetHorizontalDelta(int x1, int x2)
    {
        var dx = x2 - x1;

        if (dx > 0)
        {
            if (dx + dx > LevelWidth)
                return dx - LevelWidth;

            return dx;
        }

        if (dx + dx < -LevelWidth)
            return dx + LevelWidth;

        return dx;
    }
}