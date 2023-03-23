using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class UpOrientation : IOrientation
{
    public static UpOrientation Instance { get; } = new();

    private UpOrientation()
    {
    }

    public int RotNum => 2;

    public LevelPosition MoveRight(LevelPosition position, int step)
    {
        position.X -= step;
        return position;
    }

    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        position.Y += step;
        return position;
    }

    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        position.X += step;
        return position;
    }

    public LevelPosition MoveDown(LevelPosition position, int step)
    {
        position.Y -= step;
        return position;
    }

    public LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return new LevelPosition(
            position.X - relativeDirection.X,
            position.Y + relativeDirection.Y);
    }

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