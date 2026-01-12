using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data;

public readonly struct StyleIdentifier : IEquatable<StyleIdentifier>
{
    private readonly string _styleIdentifier;

    public StyleIdentifier(string? styleIdentifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(styleIdentifier);

        _styleIdentifier = styleIdentifier;
    }

    public override string ToString() => _styleIdentifier;

    public bool Equals(StyleIdentifier other) => string.Equals(_styleIdentifier, other._styleIdentifier, StringComparison.Ordinal);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is StyleIdentifier other && Equals(other);
    public override int GetHashCode() => _styleIdentifier.GetHashCode();
    public static bool operator ==(StyleIdentifier left, StyleIdentifier right) => left.Equals(right);
    public static bool operator !=(StyleIdentifier left, StyleIdentifier right) => !left.Equals(right);
}
