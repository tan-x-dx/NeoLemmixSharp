using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Level;

public readonly struct LevelVersion(ulong levelVersion) : IEquatable<LevelVersion>
{
    public readonly ulong Version = levelVersion;

    public bool Equals(LevelVersion other) => Version == other.Version;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is LevelVersion other && Equals(other);
    public override int GetHashCode() => Version.GetHashCode();
    public static bool operator ==(LevelVersion left, LevelVersion right) => left.Equals(right);
    public static bool operator !=(LevelVersion left, LevelVersion right) => !left.Equals(right);
}
