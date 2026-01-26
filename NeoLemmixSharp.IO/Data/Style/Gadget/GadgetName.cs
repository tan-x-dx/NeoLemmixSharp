using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public readonly struct GadgetName : IEquatable<GadgetName>
{
    private readonly string _gadgetName;

    public GadgetName(string? gadgetName)
    {
        _gadgetName = gadgetName ?? string.Empty;
    }

    public static GadgetName Empty => new(string.Empty);

    public bool IsTrivial => string.IsNullOrWhiteSpace(_gadgetName);
    public override string ToString() => _gadgetName;

    public bool Equals(GadgetName other) => string.Equals(_gadgetName, other._gadgetName, StringComparison.Ordinal);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetName other && Equals(other);
    public override int GetHashCode() => _gadgetName.GetHashCode();
    public static bool operator ==(GadgetName left, GadgetName right) => left.Equals(right);
    public static bool operator !=(GadgetName left, GadgetName right) => !left.Equals(right);
}
