using NeoLemmixSharp.Common.Util;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.Orientations;

public sealed class RightOrientation : Orientation
{
    public static RightOrientation Instance { get; } = new();

    private RightOrientation() : base(3, 1, 0)
    {
    }

    [Pure]
    public override LevelPosition TopLeftCornerOfLevel() => new(0, Terrain.Height);
    [Pure]
    public override LevelPosition TopRightCornerOfLevel() => new(0, 0);
    [Pure]
    public override LevelPosition BottomLeftCornerOfLevel() => new(Terrain.Width, Terrain.Height);
    [Pure]
    public override LevelPosition BottomRightCornerOfLevel() => new(Terrain.Width, 0);

    [Pure]
    public override LevelPosition MoveRight(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    [Pure]
    public override LevelPosition MoveUp(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    [Pure]
    public override LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    [Pure]
    public override LevelPosition MoveDown(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    [Pure]
    public override LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - relativeDirection.Y, position.Y - relativeDirection.X));
    }

    [Pure]
    public override LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - dy, position.Y - dx));
    }

    [Pure]
    public override bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    [Pure]
    public override bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    [Pure]
    public override bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;
    [Pure]
    public override bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    [Pure]
    public override bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;
    [Pure]
    public override bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;

    [Pure]
    public override Orientation RotateClockwise() => DownOrientation.Instance;
    [Pure]
    public override Orientation RotateCounterClockwise() => UpOrientation.Instance;
    [Pure]
    public override Orientation GetOpposite() => LeftOrientation.Instance;

    public override string ToString() => "right";
}