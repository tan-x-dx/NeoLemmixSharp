using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class DownOrientation : IOrientation
{
    public static DownOrientation Instance { get; } = new();

    private DownOrientation()
    {
    }

    public int RotNum => 0;

    public LevelPosition MoveRight(LevelPosition position, int step)
    {
        position.X += step;
        return position;
    }

    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        position.Y -= step;
        return position;
    }

    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        position.X -= step;
        return position;
    }

    public LevelPosition MoveDown(LevelPosition position, int step)
    {
        position.Y += step;
        return position;
    }

    public LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return new LevelPosition(
            position.X + relativeDirection.X,
            position.Y - relativeDirection.Y);
    }

    public ActionSprite GetActionSprite(LemmingActionSpriteBundle actionSpriteBundle, IFacingDirection facingDirection)
    {
        var left = actionSpriteBundle.DownLeftSprite;
        var right = actionSpriteBundle.DownRightSprite;

        return facingDirection.ChooseActionSprite(left, right);
    }

    public void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite)
    {
        actionSpriteBundle.DownLeftSprite = leftSprite;
    }

    public void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite)
    {
        actionSpriteBundle.DownRightSprite = rightSprite;
    }

    public bool Equals(IOrientation? other) => other is DownOrientation;
    public override bool Equals(object? obj) => obj is DownOrientation;
    public override int GetHashCode() => nameof(DownOrientation).GetHashCode();

    public override string ToString() => "down";
}