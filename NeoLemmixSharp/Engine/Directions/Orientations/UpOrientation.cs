using Microsoft.Xna.Framework;
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
    public Point TopLeftCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, LevelScreen.CurrentLevel.Height);
    public Point TopRightCornerOfLevel() => new(0, LevelScreen.CurrentLevel.Height);
    public Point BottomLeftCornerOfLevel() => new(LevelScreen.CurrentLevel.Width, 0);
    public Point BottomRightCornerOfLevel() => new(0, 0);

    public Point MoveRight(Point position, int step)
    {
        position.X -= step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveUp(Point position, int step)
    {
        position.Y += step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveLeft(Point position, int step)
    {
        position.X += step;
        return Terrain.NormalisePosition(position);
    }

    public Point MoveDown(Point position, int step)
    {
        position.Y -= step;
        return Terrain.NormalisePosition(position);
    }

    public Point Move(Point position, Point relativeDirection)
    {
        return Terrain.NormalisePosition(new Point(
            position.X - relativeDirection.X,
            position.Y + relativeDirection.Y));
    }

    public Point Move(Point position, int dx, int dy)
    {
        position.X -= dx;
        position.Y += dy;

        return Terrain.NormalisePosition(position);
    }

    public bool MatchesHorizontally(Point firstPosition, Point secondPosition) => firstPosition.X == secondPosition.X;
    public bool MatchesVertically(Point firstPosition, Point secondPosition) => firstPosition.Y == secondPosition.Y;
    public bool FirstIsAboveSecond(Point firstPosition, Point secondPosition) => firstPosition.Y > secondPosition.Y;
    public bool FirstIsBelowSecond(Point firstPosition, Point secondPosition) => firstPosition.Y < secondPosition.Y;
    public bool FirstIsToLeftOfSecond(Point firstPosition, Point secondPosition) => firstPosition.X > secondPosition.X;
    public bool FirstIsToRightOfSecond(Point firstPosition, Point secondPosition) => firstPosition.X < secondPosition.X;

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