﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.FacingDirections;

public abstract class FacingDirection : IEquatable<FacingDirection>, IUniqueIdItem
{
    private static readonly FacingDirection[] FacingDirections = GenerateFacingDirectionCollection();

    public static ReadOnlySpan<FacingDirection> AllFacingDirections => new(FacingDirections);

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

public sealed class SimpleFacingDirectionHasher : ISimpleHasher<FacingDirection>
{
    public static SimpleFacingDirectionHasher Instance { get; } = new();

    private SimpleFacingDirectionHasher()
    {
    }

    public int NumberOfItems => FacingDirection.AllFacingDirections.Length;

    public int Hash(FacingDirection facingDirection) => facingDirection.Id;
    public FacingDirection Unhash(int hash) => FacingDirection.AllFacingDirections[hash];
}