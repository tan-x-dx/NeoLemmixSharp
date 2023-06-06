using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering.LevelRendering;
using System;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public abstract class FacingDirection : IEquatable<FacingDirection>
{
    public abstract int DeltaX { get; }
    public abstract FacingDirection OppositeDirection { get; }
    public abstract ActionSprite ChooseActionSprite(LemmingActionSpriteBundle actionSpriteBundle, Orientation orientation);
    public abstract Orientation ConvertToRelativeOrientation(Orientation orientation);

    public bool Equals(FacingDirection? other) => DeltaX == (other?.DeltaX ?? 0);
    public sealed override bool Equals(object? obj) => obj is FacingDirection other && DeltaX == other.DeltaX;
    public sealed override int GetHashCode() => DeltaX;
    public abstract override string ToString();

    public static bool operator ==(FacingDirection left, FacingDirection right) => left.DeltaX == right.DeltaX;
    public static bool operator !=(FacingDirection left, FacingDirection right) => left.DeltaX != right.DeltaX;
}