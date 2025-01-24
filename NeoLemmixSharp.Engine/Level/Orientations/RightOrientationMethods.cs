using NeoLemmixSharp.Common.Util;
using System.Diagnostics.Contracts;
using static NeoLemmixSharp.Engine.Level.LevelScreen;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public static class RightOrientationMethods
{
    [Pure]
    public static LevelPosition MoveRight(LevelPosition position, int step)
    {
        return new LevelPosition(position.X, position.Y - step);
    }

    [Pure]
    public static LevelPosition MoveUp(LevelPosition position, int step)
    {
        return new LevelPosition(position.X - step, position.Y);
    }

    [Pure]
    public static LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return new LevelPosition(position.X, position.Y + step);
    }

    [Pure]
    public static LevelPosition MoveDown(LevelPosition position, int step)
    {
        return new LevelPosition(position.X + step, position.Y);
    }

    [Pure]
    public static LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return new LevelPosition(position.X - relativeDirection.Y, position.Y - relativeDirection.X);
    }

    [Pure]
    public static LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return new LevelPosition(position.X - dy, position.Y - dx);
    }

    [Pure]
    public static bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    [Pure]
    public static bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    [Pure]
    public static bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;
    [Pure]
    public static bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    [Pure]
    public static bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;
    [Pure]
    public static bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;

    [Pure]
    public static int GetHorizontalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        var a = fromPosition.Y;
        var b = toPosition.Y;

        return VerticalBoundaryBehaviour.GetDelta(b, a);
    }

    [Pure]
    public static int GetVerticalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        var a = fromPosition.X;
        var b = toPosition.X;

        return HorizontalBoundaryBehaviour.GetDelta(a, b);
    }
}