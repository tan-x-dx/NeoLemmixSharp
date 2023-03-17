namespace NeoLemmixSharp.Engine.Directions.Orientations;

public sealed class RightOrientation : IOrientation
{
    public static RightOrientation Instance { get; } = new();

    private RightOrientation()
    {
    }

    public LevelPosition MoveRight(LevelPosition position, int step)
    {
        position.Y += step;
        return position;
    }

    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        position.X -= step;
        return position;
    }

    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        position.Y -= step;
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
            position.Y + relativeDirection.X);
    }

    public bool Equals(IOrientation? other) => other is RightOrientation;
    public override bool Equals(object? obj) => obj is RightOrientation;
    public override int GetHashCode() => nameof(RightOrientation).GetHashCode();

    public override string ToString() => "Right";
}