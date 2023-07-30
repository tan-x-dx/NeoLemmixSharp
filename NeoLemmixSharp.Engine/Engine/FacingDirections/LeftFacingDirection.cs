using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.FacingDirections;

public sealed class LeftFacingDirection : FacingDirection
{
    public static LeftFacingDirection Instance { get; } = new();

    private LeftFacingDirection()
    {
    }

    public override int DeltaX => -1;
    public override int Id => 1;

    [Pure]
    public override FacingDirection OppositeDirection() => RightFacingDirection.Instance;
    [Pure]
    public override Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.RotateClockwise();

    public override string ToString() => "left";
}