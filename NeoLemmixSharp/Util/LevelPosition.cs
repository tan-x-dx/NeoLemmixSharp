using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Util;

public readonly struct LevelPosition : IEquatable<LevelPosition>
{
    public readonly int X;
    public readonly int Y;

    public LevelPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(LevelPosition left, LevelPosition right) =>
        left.X == right.X &&
        left.Y == right.Y;

    public static bool operator !=(LevelPosition left, LevelPosition right) =>
        left.X != right.X ||
        left.Y != right.Y;

    public static LevelPosition operator +(LevelPosition left, LevelPosition right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static LevelPosition operator -(LevelPosition left, LevelPosition right) =>
        new(left.X - right.X, left.Y - right.Y);

    public bool Equals(LevelPosition other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is LevelPosition other && X == other.X && Y == other.Y;
    public override int GetHashCode() => 3790121 * X +
                                         2885497 * Y +
                                         1088251;

    public override string ToString() => $"[{X},{Y}]";
}

public sealed class LevelPositionEqualityComparer : IEqualityComparer<LevelPosition>, IEquatable<LevelPositionEqualityComparer>
{
    public static LevelPositionEqualityComparer Instance { get; } = new();

    private LevelPositionEqualityComparer()
    {
    }

    public bool Equals(LevelPosition left, LevelPosition right) => left.X == right.X &&
                                                                   left.Y == right.Y;

    public int GetHashCode(LevelPosition levelPosition) => 3790121 * levelPosition.X +
                                                           2885497 * levelPosition.Y +
                                                           1088251;

    bool IEquatable<LevelPositionEqualityComparer>.Equals(LevelPositionEqualityComparer? other) => ReferenceEquals(other, Instance);
    public override bool Equals(object? obj) => ReferenceEquals(obj, Instance);
    public override int GetHashCode() => nameof(LevelPositionEqualityComparer).GetHashCode();
}