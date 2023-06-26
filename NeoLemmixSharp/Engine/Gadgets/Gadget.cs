using System;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Gadgets;

public abstract class Gadget : IEquatable<Gadget>
{
    public abstract GadgetType GadgetType { get; }
    public abstract int GadgetId { get; }

    public abstract bool CanActAsSolid { get; }
    public abstract bool CanActAsIndestructible { get; }

    public abstract bool IsSolidToLemming(LevelPosition levelPosition, Lemming lemming);
    public abstract bool IsIndestructibleToLemming(LevelPosition levelPosition, Lemming lemming);

    public abstract bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation);

    public bool Equals(Gadget? other) => other is not null && GadgetId == other.GadgetId;
    public sealed override bool Equals(object? obj) => obj is Gadget other && GadgetId == other.GadgetId;
    public sealed override int GetHashCode() => GadgetId;

    public static bool operator ==(Gadget left, Gadget right) => left.GadgetId == right.GadgetId;
    public static bool operator !=(Gadget left, Gadget right) => left.GadgetId != right.GadgetId;
}