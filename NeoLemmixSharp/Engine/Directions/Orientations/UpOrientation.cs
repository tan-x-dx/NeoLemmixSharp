using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.Directions.Orientations.IOrientation;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class UpOrientation : IOrientation
{
    public static UpOrientation Instance { get; } = new();

    private UpOrientation()
    {
    }

    public int RotNum => 2;
    public LevelPosition TopLeftCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, LevelScreen.CurrentLevel.Height);
    public LevelPosition TopRightCornerOfLevel() => new(0, LevelScreen.CurrentLevel.Height);
    public LevelPosition BottomLeftCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, 0);
    public LevelPosition BottomRightCornerOfLevel() => new(0, 0);

    public LevelPosition MoveRight(LevelPosition position, int step)
    {
        position.X -= step;
        return Terrain.NormalisePosition(position);
    }

    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        position.Y += step;
        return Terrain.NormalisePosition(position);
    }

    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        position.X += step;
        return Terrain.NormalisePosition(position);
    }

    public LevelPosition MoveDown(LevelPosition position, int step)
    {
        position.Y -= step;
        return Terrain.NormalisePosition(position);
    }

    public LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(
            position.X - relativeDirection.X,
            position.Y + relativeDirection.Y));
    }

    public LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        position.X -= dx;
        position.Y += dy;

        return Terrain.NormalisePosition(position);
    }

    public bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X == secondPosition.X;
    public bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y == secondPosition.Y;
    public bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y > secondPosition.Y;
    public bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.Y < secondPosition.Y;
    public bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X > secondPosition.X;
    public bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => firstPosition.X < secondPosition.X;

    public ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.UpLeftSprite;
    }

    public ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.UpRightSprite;
    }

    public void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite)
    {
        actionSpriteBundle.UpLeftSprite = leftSprite;
    }

    public void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite)
    {
        actionSpriteBundle.UpRightSprite = rightSprite;
    }

    public bool Equals(IOrientation? other) => other is UpOrientation;
    public override bool Equals(object? obj) => obj is UpOrientation;
    public override int GetHashCode() => nameof(UpOrientation).GetHashCode();

    public override string ToString() => "up";
}