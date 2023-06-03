namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public sealed class VerticalWrapBoundaryBehaviour : IVerticalBoundaryBehaviour
{
    private readonly int _levelHeightInPixels;

    public VerticalWrapBoundaryBehaviour(int levelHeightInPixels)
    {
        _levelHeightInPixels = levelHeightInPixels;
    }

    public int NormaliseY(int y)
    {
        // most likely case for negatives will be "small" numbers. Therefore simply adding the level height will make it a valid value
        if (y < 0)
            return y + _levelHeightInPixels;

        if (y < _levelHeightInPixels)
            return y;

        // most likely case for "big" numbers will be less than twice the level height. Therefore simply subtracting the level height will make it a valid value
        y -= _levelHeightInPixels;

        if (y < _levelHeightInPixels)
            return y;

        // otherwise, just do modulo operation
        return y % _levelHeightInPixels;
    }

    public int GetVerticalDistanceSquared(int y1, int y2)
    {
        y1 = NormaliseY(y1);
        y2 = NormaliseY(y2);
        var dy = y2 - y1;
        return dy * dy;
    }
}