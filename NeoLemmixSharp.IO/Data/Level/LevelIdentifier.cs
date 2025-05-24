using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data.Level;

public readonly struct LevelIdentifier(ulong levelId) : IEquatable<LevelIdentifier>
{
    public readonly ulong LevelId = levelId;

    public bool Equals(LevelIdentifier other) => LevelId == other.LevelId;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is LevelIdentifier other && Equals(other);
    public override int GetHashCode() => LevelId.GetHashCode();
    public static bool operator ==(LevelIdentifier left, LevelIdentifier right) => left.Equals(right);
    public static bool operator !=(LevelIdentifier left, LevelIdentifier right) => !left.Equals(right);
}
