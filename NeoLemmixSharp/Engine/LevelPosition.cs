using System;

namespace NeoLemmixSharp.Engine;

public struct LevelPosition : IEquatable<LevelPosition>
{
    public int X;
    public int Y;

    public LevelPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static LevelPosition operator +(LevelPosition left, LevelPosition right) => new(left.X + right.X, left.Y + right.Y);
    public static LevelPosition operator -(LevelPosition left, LevelPosition right) => new(left.X - right.X, left.Y - right.Y);

    public bool Equals(LevelPosition other) =>
        X == other.X &&
        Y == other.Y;
    public override bool Equals(object? obj) => obj is LevelPosition other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public static bool operator ==(LevelPosition left, LevelPosition right) => left.Equals(right);
    public static bool operator !=(LevelPosition left, LevelPosition right) => !left.Equals(right);
}