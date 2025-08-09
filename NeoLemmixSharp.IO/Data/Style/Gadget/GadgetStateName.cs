using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public readonly struct GadgetStateName : IEquatable<GadgetStateName>
{
    private readonly string _gadgetStateName;

    public GadgetStateName(string? gadgetStateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gadgetStateName);

        _gadgetStateName = gadgetStateName;
    }

    public override string ToString() => _gadgetStateName;

    public bool Equals(GadgetStateName other) => string.Equals(_gadgetStateName, other._gadgetStateName);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetStateName other && Equals(other);
    public override int GetHashCode() => _gadgetStateName.GetHashCode();
    public static bool operator ==(GadgetStateName left, GadgetStateName right) => left.Equals(right);
    public static bool operator !=(GadgetStateName left, GadgetStateName right) => !left.Equals(right);
}
