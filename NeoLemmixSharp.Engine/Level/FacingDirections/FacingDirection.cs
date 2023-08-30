using System.Diagnostics.Contracts;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.FacingDirections;

public abstract class FacingDirection : IUniqueIdItem<FacingDirection>
{
    private static readonly FacingDirection[] FacingDirections = GenerateFacingDirectionCollection();

    public static int NumberOfItems => FacingDirections.Length;
    public static ReadOnlySpan<FacingDirection> AllItems => new(FacingDirections);

    private static FacingDirection[] GenerateFacingDirectionCollection()
    {
        var facingDirections = new FacingDirection[2];

        facingDirections[RightFacingDirection.Instance.Id] = RightFacingDirection.Instance;
        facingDirections[LeftFacingDirection.Instance.Id] = LeftFacingDirection.Instance;

        facingDirections.ValidateUniqueIds();

        return facingDirections;
    }

    public abstract int DeltaX { get; }
    public abstract int Id { get; }

    [Pure]
    public abstract FacingDirection OppositeDirection();
    [Pure]
    public abstract Orientation ConvertToRelativeOrientation(Orientation orientation);

    public bool Equals(FacingDirection? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is FacingDirection other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public abstract override string ToString();

    public static bool operator ==(FacingDirection left, FacingDirection right) => left.Id == right.Id;
    public static bool operator !=(FacingDirection left, FacingDirection right) => left.Id != right.Id;
}