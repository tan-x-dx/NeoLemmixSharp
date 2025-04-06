using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public static class DownOrientationMethods
{
    [Pure]
    public static Point MoveRight(Point position, int step) => new(position.X + step, position.Y);

    [Pure]
    public static Point MoveUp(Point position, int step) => new(position.X, position.Y - step);

    [Pure]
    public static Point MoveLeft(Point position, int step) => new(position.X - step, position.Y);

    [Pure]
    public static Point MoveDown(Point position, int step) => new(position.X, position.Y + step);

    [Pure]
    public static Point Move(Point position, int dx, int dy) => new(position.X + dx, position.Y - dy);

    [Pure]
    public static bool MatchesHorizontally(Point firstPosition, Point secondPosition) => firstPosition.X == secondPosition.X;
    [Pure]
    public static bool MatchesVertically(Point firstPosition, Point secondPosition) => firstPosition.Y == secondPosition.Y;
    [Pure]
    public static bool FirstIsAboveSecond(Point firstPosition, Point secondPosition) => firstPosition.Y < secondPosition.Y;
    [Pure]
    public static bool FirstIsBelowSecond(Point firstPosition, Point secondPosition) => firstPosition.Y > secondPosition.Y;
    [Pure]
    public static bool FirstIsToLeftOfSecond(Point firstPosition, Point secondPosition) => firstPosition.X < secondPosition.X;
    [Pure]
    public static bool FirstIsToRightOfSecond(Point firstPosition, Point secondPosition) => firstPosition.X > secondPosition.X;

    [Pure]
    public static int GetHorizontalDelta(Point fromPosition, Point toPosition)
    {
        var a = fromPosition.X;
        var b = toPosition.X;

        return LevelScreen.HorizontalBoundaryBehaviour.GetDelta(a, b);
    }

    [Pure]
    public static int GetVerticalDelta(Point fromPosition, Point toPosition)
    {
        var a = fromPosition.Y;
        var b = toPosition.Y;

        return LevelScreen.VerticalBoundaryBehaviour.GetDelta(a, b);
    }
}