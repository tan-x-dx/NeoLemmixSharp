﻿using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Orientations;

public sealed class RightOrientation : Orientation
{
    public static RightOrientation Instance { get; } = new();

    private RightOrientation()
    {
    }

    public override int RotNum => 3;
    public override int AbsoluteHorizontalComponent => 1;
    public override int AbsoluteVerticalComponent => 0;

    public override LevelPosition TopLeftCornerOfLevel() => new(0, Terrain.Height);
    public override LevelPosition TopRightCornerOfLevel() => new(0, 0);
    public override LevelPosition BottomLeftCornerOfLevel() => new(Terrain.Width, Terrain.Height);
    public override LevelPosition BottomRightCornerOfLevel() => new(Terrain.Width, 0);

    public override LevelPosition MoveRight(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    public override LevelPosition MoveUp(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    public override LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    public override LevelPosition MoveDown(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    public override LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - relativeDirection.Y, position.Y - relativeDirection.X));
    }

    public override LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - dy, position.Y - dx));
    }

    public override bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    public override bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    public override bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;
    public override bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    public override bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;
    public override bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;

    public override Orientation RotateClockwise() => DownOrientation.Instance;
    public override Orientation RotateCounterClockwise() => UpOrientation.Instance;
    public override Orientation GetOpposite() => LeftOrientation.Instance;

    public override string ToString() => "right";
}