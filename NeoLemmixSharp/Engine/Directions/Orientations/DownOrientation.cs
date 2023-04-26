using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class DownOrientation : Orientation
{
    public static DownOrientation Instance { get; } = new();

    private DownOrientation()
    {
    }

    public override int RotNum => 0;
    public override LevelPosition TopLeftCornerOfLevel() => new(0, 0);
    public override LevelPosition TopRightCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, 0);
    public override LevelPosition BottomLeftCornerOfLevel() => new(0, LevelScreen.CurrentLevel.Height);
    public override LevelPosition BottomRightCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, LevelScreen.CurrentLevel.Height);

    public override LevelPosition MoveRight(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + step, position.Y));
    }

    public override LevelPosition MoveUp(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    public override LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    public override LevelPosition MoveDown(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    public override LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + relativeDirection.X, position.Y - relativeDirection.Y));
    }

    public override LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + dx, position.Y - dy));
    }

    public override bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    public override bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    public override bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;
    public override bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;
    public override bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;
    public override bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;

    public override ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.DownLeftSprite;
    }

    public override ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.DownRightSprite;
    }

    public override void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite)
    {
        actionSpriteBundle.DownLeftSprite = leftSprite;
    }

    public override void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite)
    {
        actionSpriteBundle.DownRightSprite = rightSprite;
    }

    public override string ToString() => "down";
}