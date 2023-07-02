namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public sealed class HorizontalWrapBoundaryBehaviour : IHorizontalBoundaryBehaviour
{
    private readonly int _levelWidthInPixels;

    public HorizontalWrapBoundaryBehaviour(int levelWidthInPixels)
    {
        _levelWidthInPixels = levelWidthInPixels;
    }

    public int NormaliseX(int x)
    {
        // most likely case for negatives will be "small" numbers. Therefore simply adding the level width will make it a valid value
        if (x < 0)
            return x + _levelWidthInPixels;

        if (x < _levelWidthInPixels)
            return x;

        // most likely case for "big" numbers will be less than twice the level width. Therefore simply subtracting the level width will make it a valid value
        x -= _levelWidthInPixels;

        if (x < _levelWidthInPixels)
            return x;

        // otherwise, just do modulo operation
        return x % _levelWidthInPixels;
    }

    public int GetAbsoluteHorizontalDistance(int x1, int x2)
    {
        var dx = Math.Abs(x1 - x2);
        if (dx + dx > _levelWidthInPixels)
            return _levelWidthInPixels - dx;

        return dx;
    }
}