using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public readonly struct TribeStyleIdentifier(StyleIdentifier styleIdentifier, int themeTribeId) : IEquatable<TribeStyleIdentifier>
{
    public readonly StyleIdentifier StyleIdentifier = styleIdentifier;
    public readonly int ThemeTribeId = themeTribeId;

    public bool Equals(TribeStyleIdentifier other) => ThemeTribeId == other.ThemeTribeId && StyleIdentifier.Equals(other.StyleIdentifier);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is TribeStyleIdentifier other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(StyleIdentifier, ThemeTribeId);
    public static bool operator ==(TribeStyleIdentifier left, TribeStyleIdentifier right) => left.Equals(right);
    public static bool operator !=(TribeStyleIdentifier left, TribeStyleIdentifier right) => !left.Equals(right);
}
