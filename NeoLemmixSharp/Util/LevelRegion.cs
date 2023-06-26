using System;

namespace NeoLemmixSharp.Util;

public readonly struct LevelRegion : IEquatable<LevelRegion>
{
    public readonly int X;
    public readonly int Y;
    public readonly int W;
    public readonly int H;

    public int X1 => X + W;
    public int Y1 => Y + H;

    public LevelRegion(int x, int y, int w, int h)
    {
        X = x;
        Y = y;
        W = w;
        H = h;
    }

    public bool ContainsPoint(LevelPosition levelPosition) => X <= levelPosition.X &&
                                                              Y <= levelPosition.Y &&
                                                              levelPosition.X < X1 &&
                                                              levelPosition.Y < Y1;

    public bool IntersectsRegion(LevelRegion otherRegion) => otherRegion.X < X1 &&
                                                             X < otherRegion.X1 &&
                                                             otherRegion.Y < Y1 &&
                                                             Y < otherRegion.Y1;

    public bool Equals(LevelRegion other) => X == other.X &&
                                             Y == other.Y &&
                                             W == other.W &&
                                             H == other.H;

    public override bool Equals(object? obj) => obj is LevelRegion other &&
                                                X == other.X &&
                                                Y == other.Y &&
                                                W == other.W &&
                                                H == other.H;

    public override int GetHashCode() => 79427 * X +
                                         63391 * Y +
                                         24821 * W +
                                         39097 * H +
                                         57719;

    public static bool operator ==(LevelRegion left, LevelRegion right) => left.X == right.X &&
                                                                           left.Y == right.Y &&
                                                                           left.W == right.W &&
                                                                           left.H == right.H;

    public static bool operator !=(LevelRegion left, LevelRegion right) => left.X != right.X ||
                                                                           left.Y != right.Y ||
                                                                           left.W != right.W ||
                                                                           left.H != right.H;
}