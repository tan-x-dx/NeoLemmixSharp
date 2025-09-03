using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Level.Gadget;

[DebuggerDisplay("GadgetId: {GadgetId}")]
public readonly struct GadgetIdentifier(int gadgetId) : IEquatable<GadgetIdentifier>
{
    public readonly int GadgetId = gadgetId;

    public bool Equals(GadgetIdentifier other) => GadgetId == other.GadgetId;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetIdentifier other && Equals(other);
    public override int GetHashCode() => GadgetId;
    public static bool operator ==(GadgetIdentifier left, GadgetIdentifier right) => left.Equals(right);
    public static bool operator !=(GadgetIdentifier left, GadgetIdentifier right) => !left.Equals(right);
}
