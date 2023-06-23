using NeoLemmixSharp.Engine.Directions.Orientations;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public sealed class RightFacingDirection : FacingDirection
{
    public static RightFacingDirection Instance { get; } = new();

    private RightFacingDirection()
    {
    }

    public override int DeltaX => 1;
    public override int FacingId => 0;

    public override FacingDirection OppositeDirection => LeftFacingDirection.Instance;
    public override Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.RotateCounterClockwise();

    public override string ToString() => "right";
}