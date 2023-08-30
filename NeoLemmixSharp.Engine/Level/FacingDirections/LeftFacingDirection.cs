using System.Diagnostics.Contracts;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public sealed class LeftFacingDirection : FacingDirection
{
    public static LeftFacingDirection Instance { get; } = new();

    private LeftFacingDirection()
    {
    }

    public override int DeltaX => -1;
    public override int Id => GameConstants.LeftFacingDirectionId;

    [Pure]
    public override FacingDirection OppositeDirection() => RightFacingDirection.Instance;
    [Pure]
    public override Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.RotateClockwise();

    public override string ToString() => "left";
}