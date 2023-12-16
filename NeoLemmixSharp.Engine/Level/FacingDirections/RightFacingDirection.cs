using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public sealed class RightFacingDirection : FacingDirection
{
    public static readonly RightFacingDirection Instance = new();

    private RightFacingDirection()
    {
    }

    public override int DeltaX => 1;
    public override int Id => LevelConstants.RightFacingDirectionId;

    [Pure]
    public override FacingDirection GetOpposite() => LeftFacingDirection.Instance;
    [Pure]
    public override Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.RotateCounterClockwise();

    public override string ToString() => "right";
}