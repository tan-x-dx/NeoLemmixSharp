using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public static class OrientationMethods
{
    [Pure]
    public static LevelPosition MoveDown(
        this Orientation orientation,
        LevelPosition position,
        int step)
    {
        var newPosition = orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MoveDown(position, step),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MoveDown(position, step),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MoveDown(position, step),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MoveDown(position, step),

            _ => Orientation.ThrowOrientationOutOfRangeException<LevelPosition>(orientation)
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    [Pure]
    public static LevelPosition MoveLeft(
        this Orientation orientation,
        LevelPosition position,
        int step)
    {
        var newPosition = orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MoveLeft(position, step),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MoveLeft(position, step),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MoveLeft(position, step),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MoveLeft(position, step),

            _ => Orientation.ThrowOrientationOutOfRangeException<LevelPosition>(orientation)
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    [Pure]
    public static LevelPosition MoveUp(
        this Orientation orientation,
        LevelPosition position,
        int step)
    {
        var newPosition = orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MoveUp(position, step),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MoveUp(position, step),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MoveUp(position, step),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MoveUp(position, step),

            _ => Orientation.ThrowOrientationOutOfRangeException<LevelPosition>(orientation)
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    [Pure]
    public static LevelPosition MoveRight(
        this Orientation orientation,
        LevelPosition position,
        int step)
    {
        var newPosition = orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MoveRight(position, step),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MoveRight(position, step),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MoveRight(position, step),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MoveRight(position, step),

            _ => Orientation.ThrowOrientationOutOfRangeException<LevelPosition>(orientation)
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    [Pure]
    public static LevelPosition Move(
        this Orientation orientation,
        LevelPosition position,
        int dx,
        int dy)
    {
        var newPosition = orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.Move(position, dx, dy),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.Move(position, dx, dy),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.Move(position, dx, dy),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.Move(position, dx, dy),

            _ => Orientation.ThrowOrientationOutOfRangeException<LevelPosition>(orientation)
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    [Pure]
    public static LevelPosition MoveWithoutNormalization(
        this Orientation orientation,
        LevelPosition position,
        int dx,
        int dy) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.Move(position, dx, dy),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.Move(position, dx, dy),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.Move(position, dx, dy),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.Move(position, dx, dy),

            _ => Orientation.ThrowOrientationOutOfRangeException<LevelPosition>(orientation)
        };

    [Pure]
    public static bool MatchesHorizontally(
        this Orientation orientation,
        LevelPosition firstPosition,
        LevelPosition secondPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<bool>(orientation)
        };

    [Pure]
    public static bool MatchesVertically(
        this Orientation orientation,
        LevelPosition firstPosition,
        LevelPosition secondPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MatchesVertically(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MatchesVertically(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MatchesVertically(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MatchesVertically(firstPosition, secondPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<bool>(orientation)
        };

    [Pure]
    public static bool FirstIsAboveSecond(
        this Orientation orientation,
        LevelPosition firstPosition,
        LevelPosition secondPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<bool>(orientation)
        };

    [Pure]
    public static bool FirstIsBelowSecond(
        this Orientation orientation,
        LevelPosition firstPosition,
        LevelPosition secondPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<bool>(orientation)
        };

    [Pure]
    public static bool FirstIsToLeftOfSecond(
        this Orientation orientation,
        LevelPosition firstPosition,
        LevelPosition secondPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<bool>(orientation)
        };

    [Pure]
    public static bool FirstIsToRightOfSecond(
        this Orientation orientation,
        LevelPosition firstPosition,
        LevelPosition secondPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<bool>(orientation)
        };

    /// <summary>
    /// If the first position were to move horizontally to be in line with the second position, what is the dx it would require?
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public static int GetHorizontalDelta(
        this Orientation orientation,
        LevelPosition fromPosition,
        LevelPosition toPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<int>(orientation)
        };

    /// <summary>
    /// If the first position were to move vertically to be in line with the second position, what is the dy it would require?
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public static int GetVerticalDelta(
        this Orientation orientation,
        LevelPosition fromPosition,
        LevelPosition toPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.GetVerticalDelta(fromPosition, toPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<int>(orientation)
        };

}
