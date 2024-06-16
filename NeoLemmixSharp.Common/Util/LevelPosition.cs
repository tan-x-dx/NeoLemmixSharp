using System.Diagnostics;

namespace NeoLemmixSharp.Common.Util;

public readonly struct LevelPosition : IEquatable<LevelPosition>
{
    public readonly int X;
    public readonly int Y;

    [DebuggerStepThrough]
    public LevelPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    [DebuggerStepThrough]
    public static bool operator ==(LevelPosition left, LevelPosition right) =>
        left.X == right.X &&
        left.Y == right.Y;

    [DebuggerStepThrough]
    public static bool operator !=(LevelPosition left, LevelPosition right) =>
        left.X != right.X ||
        left.Y != right.Y;

    [DebuggerStepThrough]
    public static LevelPosition operator +(LevelPosition left, LevelPosition right) =>
        new(left.X + right.X, left.Y + right.Y);

    [DebuggerStepThrough]
    public static LevelPosition operator -(LevelPosition left, LevelPosition right) =>
        new(left.X - right.X, left.Y - right.Y);

    [DebuggerStepThrough]
    public bool Equals(LevelPosition other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is LevelPosition other && X == other.X && Y == other.Y;
    public override int GetHashCode() => 3790121 * X +
                                         2885497 * Y +
                                         1088251;

    public override string ToString() => $"[{X},{Y}]";
}