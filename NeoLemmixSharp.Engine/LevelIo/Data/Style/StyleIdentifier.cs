using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Style;

public readonly struct StyleIdentifier : IEquatable<StyleIdentifier>
{
    private readonly string? _styleName;

    public StyleIdentifier(string? styleName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(styleName);

        _styleName = styleName;
    }

    public override string ToString() => _styleName ?? string.Empty;

    public bool Equals(StyleIdentifier other) => string.Equals(_styleName, other._styleName);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is StyleIdentifier other && Equals(other);
    public override int GetHashCode() => _styleName?.GetHashCode() ?? 0;
    public static bool operator ==(StyleIdentifier left, StyleIdentifier right) => left.Equals(right);
    public static bool operator !=(StyleIdentifier left, StyleIdentifier right) => !left.Equals(right);
}
