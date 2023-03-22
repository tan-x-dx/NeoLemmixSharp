using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class RightOrientation : IOrientation
{
    public static RightOrientation Instance { get; } = new();

    private RightOrientation()
    {
    }

    public int RotNum => 3;

    public LevelPosition MoveRight(LevelPosition position, int step)
    {
        position.Y -= step;
        return position;
    }

    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        position.X -= step;
        return position;
    }

    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        position.Y += step;
        return position;
    }

    public LevelPosition MoveDown(LevelPosition position, int step)
    {
        position.X += step;
        return position;
    }

    public LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return new LevelPosition(
            position.X - relativeDirection.Y,
            position.Y - relativeDirection.X);
    }

    public ActionSprite GetActionSprite(LemmingActionSpriteBundle actionSpriteBundle, IFacingDirection facingDirection)
    {
        var left = actionSpriteBundle.RightLeftSprite;
        var right = actionSpriteBundle.RightRightSprite;

        return facingDirection.ChooseActionSprite(left, right);
    }

    public void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite)
    {
        actionSpriteBundle.RightLeftSprite = leftSprite;
    }

    public void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite)
    {
        actionSpriteBundle.RightRightSprite = rightSprite;
    }

    public bool Equals(IOrientation? other) => other is RightOrientation;
    public override bool Equals(object? obj) => obj is RightOrientation;
    public override int GetHashCode() => nameof(RightOrientation).GetHashCode();

    public override string ToString() => "right";
}