using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public readonly struct TribeIdentifier(StyleIdentifier styleIdentifier, int themeTribeId) : IEquatable<TribeIdentifier>
{
    public readonly StyleIdentifier StyleIdentifier = styleIdentifier;
    public readonly int ThemeTribeId = themeTribeId;

    public bool Equals(TribeIdentifier other) => ThemeTribeId == other.ThemeTribeId && StyleIdentifier.Equals(other.StyleIdentifier);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is TribeIdentifier other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(StyleIdentifier, ThemeTribeId);
    public static bool operator ==(TribeIdentifier left, TribeIdentifier right) => left.Equals(right);
    public static bool operator !=(TribeIdentifier left, TribeIdentifier right) => !left.Equals(right);
}
