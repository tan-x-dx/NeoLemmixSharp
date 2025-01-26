using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public sealed class FacingDirection : IExtendedEnumType<FacingDirection>
{
    public static readonly FacingDirection Right = new(
        EngineConstants.RightFacingDirectionId,
        EngineConstants.RightFacingDirectionDeltaX);
    public static readonly FacingDirection Left = new(
        EngineConstants.LeftFacingDirectionId,
        EngineConstants.LeftFacingDirectionDeltaX);

    private static readonly FacingDirection[] FacingDirections = GenerateFacingDirectionCollection();

    public static int NumberOfItems => EngineConstants.NumberOfFacingDirections;
    public static ReadOnlySpan<FacingDirection> AllItems => new(FacingDirections);

    private static FacingDirection[] GenerateFacingDirectionCollection()
    {
        var facingDirections = new FacingDirection[EngineConstants.NumberOfFacingDirections];

        facingDirections[EngineConstants.RightFacingDirectionId] = Right;
        facingDirections[EngineConstants.LeftFacingDirectionId] = Left;

        // No need for id validation here. It's just that simple

        return facingDirections;
    }

    public readonly int Id;
    public readonly int DeltaX;

    private FacingDirection(int id, int deltaX)
    {
        Id = id;
        DeltaX = deltaX;
    }

    [Pure]
    public FacingDirection GetOpposite() => FacingDirections[1 - Id];

    [Pure]
    public Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.Rotate(-DeltaX);

    int IIdEquatable<FacingDirection>.Id => Id;

    public bool Equals(FacingDirection? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is FacingDirection other && Id == other.Id;
    public override int GetHashCode() => Id;
    public override string ToString() => Id == EngineConstants.RightFacingDirectionId
        ? EngineConstants.RightFacingDirectionName
        : EngineConstants.LeftFacingDirectionName;

    public static bool operator ==(FacingDirection left, FacingDirection right) => left.Id == right.Id;
    public static bool operator !=(FacingDirection left, FacingDirection right) => left.Id != right.Id;
}