using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public sealed class LeftFacingDirection : FacingDirection
{
    public static readonly LeftFacingDirection Instance = new();

    private LeftFacingDirection()
    {
    }

    public override int DeltaX => -1;
    public override int Id => LevelConstants.LeftFacingDirectionId;

    [Pure]
    public override FacingDirection GetOpposite() => RightFacingDirection.Instance;
    [Pure]
    public override Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.RotateClockwise();

    public override string ToString() => "left";
}