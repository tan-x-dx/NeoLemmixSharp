using Microsoft.Xna.Framework;
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
    public Point TopLeftCornerOfLevel() => new(0, 0);
    public Point TopRightCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, 0);
    public Point BottomLeftCornerOfLevel() => new(0, LevelScreen.CurrentLevel.Height);
    public Point BottomRightCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, LevelScreen.CurrentLevel.Height);

    public Point MoveRight(Point position, int step)
    {
        position.X += step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveUp(Point position, int step)
    {
        position.Y -= step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveLeft(Point position, int step)
    {
        position.X -= step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveDown(Point position, int step)
    {
        position.Y += step;
        return Terrain.NormalisePosition(position);
    }

    public Point Move(Point position, Point relativeDirection)
    {
        return Terrain.NormalisePosition(new Point(
            position.X + relativeDirection.X,
            position.Y - relativeDirection.Y));
    }

    public Point Move(Point position, int dx, int dy)
    {
        position.X += dx;
        position.Y -= dy;

        return Terrain.NormalisePosition(position);
    }

    public bool MatchesHorizontally(Point firstPosition, Point secondPosition) => firstPosition.X == secondPosition.X;
    public bool MatchesVertically(Point firstPosition, Point secondPosition) => firstPosition.Y == secondPosition.Y;
    public bool FirstIsAboveSecond(Point firstPosition, Point secondPosition) => firstPosition.Y < secondPosition.Y;
    public bool FirstIsBelowSecond(Point firstPosition, Point secondPosition) => firstPosition.Y > secondPosition.Y;
    public bool FirstIsToLeftOfSecond(Point firstPosition, Point secondPosition) => firstPosition.X < secondPosition.X;
    public bool FirstIsToRightOfSecond(Point firstPosition, Point secondPosition) => firstPosition.X > secondPosition.X;

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