using NeoLemmixSharp.Engine.Directions.Orientations;
using System;

namespace NeoLemmixSharp.Engine.LevelGadgets;

public abstract class Gadget : IEquatable<Gadget>
{
    public abstract int GadgetId { get; }

    public abstract bool IsSolidToLemming(Lemming lemming);
    public abstract bool IsIndestructibleToLemming(Lemming lemming);

    public abstract bool MatchesTypeAndOrientation(GadgetType gadgetType, Orientation orientation);

    public bool Equals(Gadget? other) => other is not null && GadgetId == other.GadgetId;
    public sealed override bool Equals(object? obj) => obj is Gadget other && GadgetId == other.GadgetId;
    public sealed override int GetHashCode() => GadgetId;

    public static bool operator ==(Gadget left, Gadget right) => left.GadgetId == right.GadgetId;
    public static bool operator !=(Gadget left, Gadget right) => left.GadgetId != right.GadgetId;
}