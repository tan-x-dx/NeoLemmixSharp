using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Diagnostics.Contracts;
using static NeoLemmixSharp.Engine.Level.LevelScreen;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public sealed class UpOrientation : Orientation
{
    public static readonly UpOrientation Instance = new();

    private UpOrientation()
        : base(
            EngineConstants.UpOrientationRotNum,
            EngineConstants.UpOrientationName,
            0,
            -1)
    {
    }

    [Pure]
    public override LevelPosition MoveRight(LevelPosition position, int step)
    {
        return NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    [Pure]
    public override LevelPosition MoveUp(LevelPosition position, int step)
    {
        return NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    [Pure]
    public override LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    [Pure]
    public override LevelPosition MoveDown(LevelPosition position, int step)
    {
        return NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    [Pure]
    public override LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return NormalisePosition(new LevelPosition(position.X - relativeDirection.X, position.Y + relativeDirection.Y));
    }

    [Pure]
    public override LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return NormalisePosition(new LevelPosition(position.X - dx, position.Y + dy));
    }

    public override LevelPosition MoveWithoutNormalization(LevelPosition position, int dx, int dy)
    {
        return new LevelPosition(position.X - dx, position.Y + dy);
    }

    [Pure]
    public override bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    [Pure]
    public override bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    [Pure]
    public override bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;
    [Pure]
    public override bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;
    [Pure]
    public override bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    [Pure]
    public override bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;

    [Pure]
    public override int GetHorizontalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        var a = fromPosition.X;
        var b = toPosition.X;

        return HorizontalBoundaryBehaviour.GetDelta(b, a);
    }

    [Pure]
    public override int GetVerticalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        var a = fromPosition.Y;
        var b = toPosition.Y;

        return VerticalBoundaryBehaviour.GetDelta(b, a);
    }
}