using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Orientations;

public sealed class UpOrientation : Orientation
{
    public static UpOrientation Instance { get; } = new();

    private UpOrientation()
    {
    }

    public override int RotNum => 2;
    public override int AbsoluteHorizontalComponent => 0;
    public override int AbsoluteVerticalComponent => -1;

    public override LevelPosition TopLeftCornerOfLevel() => new(Terrain.Width, Terrain.Height);
    public override LevelPosition TopRightCornerOfLevel() => new(0, Terrain.Height);
    public override LevelPosition BottomLeftCornerOfLevel() => new(Terrain.Width, 0);
    public override LevelPosition BottomRightCornerOfLevel() => new(0, 0);

    public override LevelPosition MoveRight(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    public override LevelPosition MoveUp(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    public override LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    public override LevelPosition MoveDown(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    public override LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - relativeDirection.X, position.Y + relativeDirection.Y));
    }

    public override LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - dx, position.Y + dy));
    }

    public override bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    public override bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    public override bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;
    public override bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;
    public override bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    public override bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;

    public override Orientation RotateClockwise() => RightOrientation.Instance;

    public override Orientation RotateCounterClockwise() => LeftOrientation.Instance;
    public override Orientation GetOpposite() => DownOrientation.Instance;

    public override string ToString() => "up";
}