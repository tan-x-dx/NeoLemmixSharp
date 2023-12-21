﻿using NeoLemmixSharp.Common.Util;
using System.Diagnostics.Contracts;
using static NeoLemmixSharp.Engine.Level.LevelScreen;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public sealed class RightOrientation : Orientation
{
    public static readonly RightOrientation Instance = new();

    private RightOrientation()
    {
    }

    public override int RotNum => LevelConstants.RightOrientationRotNum;
    public override int AbsoluteHorizontalComponent => 1;
    public override int AbsoluteVerticalComponent => 0;

    [Pure]
    public override LevelPosition TopLeftCornerOfLevel() => new(0, TerrainManager.LevelHeight);
    [Pure]
    public override LevelPosition TopRightCornerOfLevel() => new(0, 0);
    [Pure]
    public override LevelPosition BottomLeftCornerOfLevel() => new(TerrainManager.LevelWidth, TerrainManager.LevelHeight);
    [Pure]
    public override LevelPosition BottomRightCornerOfLevel() => new(TerrainManager.LevelWidth, 0);

    [Pure]
    public override LevelPosition MoveRight(LevelPosition position, int step)
    {
        return TerrainManager.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    [Pure]
    public override LevelPosition MoveUp(LevelPosition position, int step)
    {
        return TerrainManager.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    [Pure]
    public override LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return TerrainManager.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    [Pure]
    public override LevelPosition MoveDown(LevelPosition position, int step)
    {
        return TerrainManager.NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    [Pure]
    public override LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return TerrainManager.NormalisePosition(new LevelPosition(position.X - relativeDirection.Y, position.Y - relativeDirection.X));
    }

    [Pure]
    public override LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return TerrainManager.NormalisePosition(new LevelPosition(position.X - dy, position.Y - dx));
    }

    public override LevelPosition MoveWithoutNormalization(LevelPosition position, int dx, int dy)
    {
        return new LevelPosition(position.X - dy, position.Y - dx);
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
    public override int GetHorizontalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        var a = fromPosition.Y;
        var b = toPosition.Y;

        return TerrainManager.VerticalBoundaryBehaviour.GetVerticalDelta(b, a);
    }

    [Pure]
    public override int GetVerticalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        var a = fromPosition.X;
        var b = toPosition.X;

        return TerrainManager.HorizontalBoundaryBehaviour.GetHorizontalDelta(a, b);
    }

    [Pure]
    public override Orientation RotateClockwise() => DownOrientation.Instance;
    [Pure]
    public override Orientation RotateCounterClockwise() => UpOrientation.Instance;
    [Pure]
    public override Orientation GetOpposite() => LeftOrientation.Instance;

    public override string ToString() => "right";
}