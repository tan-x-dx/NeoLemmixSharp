using NeoLemmixSharp.Engine.Directions.Orientations;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public interface IFacingDirection
{
    int DeltaX(int deltaX);
    IFacingDirection OppositeDirection { get; }
    LevelPosition MoveInDirection(IOrientation orientation, LevelPosition pos, int step);
}

public sealed class RightFacingDirection : IFacingDirection
{
    public static RightFacingDirection Instance { get; } = new();

    private RightFacingDirection()
    {
    }

    public int DeltaX(int deltaX) => deltaX;
    public IFacingDirection OppositeDirection => LeftFacingDirection.Instance;
    public LevelPosition MoveInDirection(IOrientation orientation, LevelPosition pos, int step) => orientation.MoveRight(pos, step);

    public override string ToString() => "Facing Right";
}

public sealed class LeftFacingDirection : IFacingDirection
{
    public static LeftFacingDirection Instance { get; } = new();

    private LeftFacingDirection()
    {
    }

    public int DeltaX(int deltaX) => -deltaX;
    public IFacingDirection OppositeDirection => RightFacingDirection.Instance;
    public LevelPosition MoveInDirection(IOrientation orientation, LevelPosition pos, int step) => orientation.MoveLeft(pos, step);

    public override string ToString() => "Facing Left";
}