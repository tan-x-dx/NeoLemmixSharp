using System;
using NeoLemmixSharp.Engine.Orientations;

namespace NeoLemmixSharp.Engine.FacingDirections;

public abstract class FacingDirection : IEquatable<FacingDirection>
{
    public abstract int DeltaX { get; }
    public abstract int Id { get; }
    public abstract FacingDirection OppositeDirection { get; }
    public abstract Orientation ConvertToRelativeOrientation(Orientation orientation);

    public bool Equals(FacingDirection? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is FacingDirection other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public abstract override string ToString();

    public static bool operator ==(FacingDirection left, FacingDirection right) => left.Id == right.Id;
    public static bool operator !=(FacingDirection left, FacingDirection right) => left.Id != right.Id;
}