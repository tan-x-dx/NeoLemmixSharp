using NeoLemmixSharp.Common.Util;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public sealed class LeftOrientation : Orientation
{
    public static LeftOrientation Instance { get; } = new();

    private LeftOrientation()
    {
    }

    public override int RotNum => GameConstants.LeftOrientationRotNum;
    public override int AbsoluteHorizontalComponent => -1;
    public override int AbsoluteVerticalComponent => 0;

    [Pure]
    public override LevelPosition TopLeftCornerOfLevel() => new(Terrain.LevelWidth, 0);
    [Pure]
    public override LevelPosition TopRightCornerOfLevel() => new(Terrain.LevelWidth, Terrain.LevelHeight);
    [Pure]
    public override LevelPosition BottomLeftCornerOfLevel() => new(0, 0);
    [Pure]
    public override LevelPosition BottomRightCornerOfLevel() => new(0, Terrain.LevelHeight);

    [Pure]
    public override LevelPosition MoveRight(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    [Pure]
    public override LevelPosition MoveUp(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    [Pure]
    public override LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    [Pure]
    public override LevelPosition MoveDown(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    [Pure]
    public override LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + relativeDirection.Y, position.Y + relativeDirection.X));
    }

    [Pure]
    public override LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + dy, position.Y + dx));
    }

    public override LevelPosition MoveWithoutNormalization(LevelPosition position, int dx, int dy)
    {
        return new LevelPosition(position.X + dy, position.Y + dx);
    }

    [Pure]
    public override bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    [Pure]
    public override bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    [Pure]
    public override bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    [Pure]
    public override bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;
    [Pure]
    public override bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;
    [Pure]
    public override bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;

    [Pure]
    public override int GetHorizontalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        var a = fromPosition.Y;
        var b = toPosition.Y;

        return Terrain.VerticalBoundaryBehaviour.GetVerticalDelta(a, b);
    }

    [Pure]
    public override int GetVerticalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        var a = fromPosition.X;
        var b = toPosition.X;

        return Terrain.HorizontalBoundaryBehaviour.GetHorizontalDelta(b, a);
    }

    [Pure]
    public override Orientation RotateClockwise() => UpOrientation.Instance;
    [Pure]
    public override Orientation RotateCounterClockwise() => DownOrientation.Instance;
    [Pure]
    public override Orientation GetOpposite() => RightOrientation.Instance;

    public override string ToString() => "left";
}