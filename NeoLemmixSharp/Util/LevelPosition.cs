﻿using System;

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

    public static bool operator ==(in LevelPosition left, in LevelPosition right) =>
        left.X == right.X &&
        left.Y == right.Y;

    public static bool operator !=(in LevelPosition left, in LevelPosition right) => 
        left.X != right.X ||
        left.Y != right.Y;

    public static LevelPosition operator +(in LevelPosition left, in LevelPosition right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static LevelPosition operator -(in LevelPosition left, in LevelPosition right) =>
        new(left.X - right.X, left.Y - right.Y);

    public bool Equals(LevelPosition other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is LevelPosition other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"[{X},{Y}]";
}