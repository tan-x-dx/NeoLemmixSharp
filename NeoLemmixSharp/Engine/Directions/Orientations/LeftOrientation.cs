using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Util;
using static NeoLemmixSharp.Engine.Directions.Orientations.IOrientation;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class LeftOrientation : IOrientation
{
    public static LeftOrientation Instance { get; } = new();

    private LeftOrientation()
    {
    }

    public int RotNum => 1;
    public LevelPosition TopLeftCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, 0);
    public LevelPosition TopRightCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, LevelScreen.CurrentLevel.Height);
    public LevelPosition BottomLeftCornerOfLevel() => new(0, 0);
    public LevelPosition BottomRightCornerOfLevel() => new(0, LevelScreen.CurrentLevel.Height);

    public LevelPosition MoveRight(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y + step));
    }

    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + step, position.Y ));
    }

    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X, position.Y - step));
    }

    public LevelPosition MoveDown(LevelPosition position, int step)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X - step, position.Y));
    }

    public LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + relativeDirection.Y, position.Y + relativeDirection.X));
    }

    public LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        return Terrain.NormalisePosition(new LevelPosition(position.X + dy, position.Y + dx));
    }

    public bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    public bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    public bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    public bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;
    public bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;
    public bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;

    public ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.LeftLeftSprite;
    }

    public ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.LeftRightSprite;
    }

    public void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite)
    {
        actionSpriteBundle.LeftLeftSprite = leftSprite;
    }

    public void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite)
    {
        actionSpriteBundle.LeftRightSprite = rightSprite;
    }

    public bool Equals(IOrientation? other) => other is LeftOrientation;
    public override bool Equals(object? obj) => obj is LeftOrientation;
    public override int GetHashCode() => nameof(LeftOrientation).GetHashCode();

    public override string ToString() => "left";
}