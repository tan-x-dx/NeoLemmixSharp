using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;

public sealed class HorizontalWrapBoundaryBehaviour : IHorizontalBoundaryBehaviour
{
    private readonly int _levelWidth;

    public BoundaryBehaviourType BoundaryBehaviourType => BoundaryBehaviourType.Wrap;
    public int LevelWidth => _levelWidth;

    public HorizontalWrapBoundaryBehaviour(int levelWidthInPixels)
    {
        _levelWidth = levelWidthInPixels;
    }

    [Pure]
    public int NormaliseX(int x)
    {
        if (x < 0)
        {
            do
            {
                x += _levelWidth;
            } while (x < 0);

            return x;
        }

        while (x >= _levelWidth)
        {
            x -= _levelWidth;
        }

        return x;
    }

    public void NormaliseXCoords(ref int left, ref int right, ref int x)
    {
        if (right < _levelWidth)
            return;

        var halfLevelWidth = _levelWidth / 2;
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
            if (dx + dx > _levelWidth)
                return dx - _levelWidth;

            return dx;
        }

        if (dx + dx < -_levelWidth)
            return dx + _levelWidth;

        return dx;
    }
}