using Microsoft.Xna.Framework;
using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.Directions.Orientations.IOrientation;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class LeftOrientation : IOrientation
{
    public static LeftOrientation Instance { get; } = new();

    private LeftOrientation()
    {
    }

    public int RotNum => 1;
    public Point TopLeftCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, 0);
    public Point TopRightCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, LevelScreen.CurrentLevel.Height);
    public Point BottomLeftCornerOfLevel() => new(0, 0);
    public Point BottomRightCornerOfLevel() => new(0, LevelScreen.CurrentLevel.Height);

    public Point MoveRight(Point position, int step)
    {
        position.Y += step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveUp(Point position, int step)
    {
        position.X += step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveLeft(Point position, int step)
    {
        position.Y -= step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveDown(Point position, int step)
    {
        position.X -= step;
        return Terrain.NormalisePosition(position);
    }

    public Point Move(Point position, Point relativeDirection)
    {
        return Terrain.NormalisePosition(new Point(
            position.X + relativeDirection.Y,
            position.Y + relativeDirection.X));
    }

    public Point Move(Point position, int dx, int dy)
    {
        position.X += dy;
        position.Y += dx;

        return Terrain.NormalisePosition(position);
    }

    public bool MatchesHorizontally(Point firstPosition, Point secondPosition) => firstPosition.Y == secondPosition.Y;
    public bool MatchesVertically(Point firstPosition, Point secondPosition) => firstPosition.X == secondPosition.X;
    public bool FirstIsAboveSecond(Point firstPosition, Point secondPosition) => firstPosition.X > secondPosition.X;
    public bool FirstIsBelowSecond(Point firstPosition, Point secondPosition) => firstPosition.X < secondPosition.X;
    public bool FirstIsToLeftOfSecond(Point firstPosition, Point secondPosition) => firstPosition.Y < secondPosition.Y;
    public bool FirstIsToRightOfSecond(Point firstPosition, Point secondPosition) => firstPosition.Y > secondPosition.Y;

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