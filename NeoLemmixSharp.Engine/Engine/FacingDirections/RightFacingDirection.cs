using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.FacingDirections;

public sealed class RightFacingDirection : FacingDirection
{
    public static RightFacingDirection Instance { get; } = new();

    private RightFacingDirection() : base(0, 1)
    {
    }

    [Pure]
    public override FacingDirection OppositeDirection() => LeftFacingDirection.Instance;
    [Pure]
    public override Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.RotateCounterClockwise();

    public override string ToString() => "right";
}