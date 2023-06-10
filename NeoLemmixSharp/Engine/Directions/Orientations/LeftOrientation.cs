using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class LeftOrientation : Orientation
{
    public static LeftOrientation Instance { get; } = new();

    private LeftOrientation()
    {
    }

    public override int RotNum => 1;
    public override int AbsoluteHorizontalComponent => -1;
    public override int AbsoluteVerticalComponent => 0;

    public override LevelPosition TopLeftCornerOfLevel() => new(Terrain.Width, 0);
    public override LevelPosition TopRightCornerOfLevel() => new(Terrain.Width, Terrain.Height);
    public override LevelPosition BottomLeftCornerOfLevel() => new(0, 0);
    public override LevelPosition BottomRightCornerOfLevel() => new(0, Terrain.Height);

    public override LevelPosition MoveRight(in LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    public override LevelPosition MoveUp(in LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    public override LevelPosition MoveLeft(in LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    public override LevelPosition MoveDown(in LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    public override LevelPosition Move(in LevelPosition position, in LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + relativeDirection.Y, position.Y + relativeDirection.X));
    }

    public override LevelPosition Move(in LevelPosition position, int dx, int dy)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + dy, position.Y + dx));
    }

    public override bool MatchesHorizontally(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    public override bool MatchesVertically(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    public override bool FirstIsAboveSecond(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    public override bool FirstIsBelowSecond(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.X < secondPosition.X;
    public override bool FirstIsToLeftOfSecond(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;
    public override bool FirstIsToRightOfSecond(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;

    public override ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.LeftLeftSprite;
    }

    public override ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.LeftRightSprite;
    }

    public override void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite)
    {
        actionSpriteBundle.LeftLeftSprite = leftSprite;
    }

    public override void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite)
    {
        actionSpriteBundle.LeftRightSprite = rightSprite;
    }

    public override Orientation RotateClockwise() => UpOrientation.Instance;
    public override Orientation RotateCounterClockwise() => DownOrientation.Instance;
    public override Orientation GetOpposite() => RightOrientation.Instance;

    public override string ToString() => "left";
}