using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class LeftOrientation : IOrientation
{
    public static LeftOrientation Instance { get; } = new();

    private LeftOrientation()
    {
    }

    public int RotNum => 1;

    public LevelPosition MoveRight(LevelPosition position, int step)
    {
        position.Y += step;
        return position;
    }

    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        position.X += step;
        return position;
    }

    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        position.Y -= step;
        return position;
    }

    public LevelPosition MoveDown(LevelPosition position, int step)
    {
        position.X -= step;
        return position;
    }

    public LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return new LevelPosition(
            position.X + relativeDirection.Y,
            position.Y + relativeDirection.X);
    }

    public LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        position.X += dy;
        position.Y += dx;

        return position;
    }

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