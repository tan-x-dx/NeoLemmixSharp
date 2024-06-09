using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;

public sealed class VerticalWrapBoundaryBehaviour : IVerticalBoundaryBehaviour
{
    private readonly int _levelHeight;

    public BoundaryBehaviourType BoundaryBehaviourType => BoundaryBehaviourType.Wrap;
    public int LevelHeight => _levelHeight;

    public VerticalWrapBoundaryBehaviour(int levelHeightInPixels)
    {
        _levelHeight = levelHeightInPixels;
    }

    [Pure]
    public int NormaliseY(int y)
    {
        if (y < 0)
        {
            do
            {
                y += _levelHeight;
            } while (y < 0);

            return y;
        }

        while (y >= _levelHeight)
        {
            y -= _levelHeight;
        }

        return y;
    }

    public void NormaliseYCoords(ref int top, ref int bottom, ref int y)
    {
        if (bottom < _levelHeight)
            return;

        var halfLevelHeight = _levelHeight / 2;
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
            if (dy + dy > _levelHeight)
                return dy - _levelHeight;

            return dy;
        }

        if (dy + dy < -_levelHeight)
            return dy + _levelHeight;

        return dy;
    }
}