using NeoLemmixSharp.Engine.Directions.FacingDirections;
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

    public ActionSprite GetActionSprite(LemmingActionSpriteBundle actionSpriteBundle, IFacingDirection facingDirection)
    {
        var left = actionSpriteBundle.LeftLeftSprite;
        var right = actionSpriteBundle.LeftRightSprite;

        return facingDirection.ChooseActionSprite(left, right);
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