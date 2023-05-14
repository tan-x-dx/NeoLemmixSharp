using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

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

    public override LevelPosition MoveRight(in LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    public override LevelPosition MoveUp(in LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    public override LevelPosition MoveLeft(in LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    public override LevelPosition MoveDown(in LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    public override LevelPosition Move(in LevelPosition position, in LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - relativeDirection.X, position.Y + relativeDirection.Y));
    }

    public override LevelPosition Move(in LevelPosition position, int dx, int dy)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - dx, position.Y + dy));
    }

    public override bool MatchesHorizontally(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    public override bool MatchesVertically(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    public override bool FirstIsAboveSecond(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;
    public override bool FirstIsBelowSecond(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;
    public override bool FirstIsToLeftOfSecond(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    public override bool FirstIsToRightOfSecond(in LevelPosition firstPosition, in LevelPosition secondPosition) => firstPosition.X < secondPosition.X;

    public override ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.UpLeftSprite;
    }

    public override ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.UpRightSprite;
    }

    public override void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite)
    {
        actionSpriteBundle.UpLeftSprite = leftSprite;
    }

    public override void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite)
    {
        actionSpriteBundle.UpRightSprite = rightSprite;
    }

    public override string ToString() => "up";
}