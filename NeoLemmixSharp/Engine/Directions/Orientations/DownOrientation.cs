using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.Directions.Orientations.IOrientation;

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
        return Terrain.NormalisePosition(position);
    }

    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        position.Y -= step;
        return Terrain.NormalisePosition(position);
    }

    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        position.X -= step;
        return Terrain.NormalisePosition(position);
    }

    public LevelPosition MoveDown(LevelPosition position, int step)
    {
        position.Y += step;
        return Terrain.NormalisePosition(position);
    }

    public LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        return Terrain.NormalisePosition(new LevelPosition(
            position.X + relativeDirection.X,
            position.Y - relativeDirection.Y));
    }

    public LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        position.X += dx;
        position.Y -= dy;

        return Terrain.NormalisePosition(position);
    }

    public ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.DownLeftSprite;
    }

    public ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle)
    {
        return actionSpriteBundle.DownRightSprite;
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