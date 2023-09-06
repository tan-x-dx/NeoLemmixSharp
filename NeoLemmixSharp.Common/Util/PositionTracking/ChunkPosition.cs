using System.Numerics;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal readonly struct ChunkPosition : IEquatable<ChunkPosition>
{
    public readonly int X;
    public readonly int Y;

    public ChunkPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"[{X}, {Y}]";

    public override bool Equals(object? obj) => obj is ChunkPosition other && X == other.X && Y == other.Y;
    public bool Equals(ChunkPosition other) => X == other.X && Y == other.Y;
    public override int GetHashCode()
    {
        var x = (uint)X;
        var y = (uint)Y;

        y = BitOperations.RotateLeft(y, 16);
        return (int)(x ^ y);
    }
}

internal sealed class ChunkPositionEqualityComparer : IEqualityComparer<ChunkPosition>, IEquatable<ChunkPositionEqualityComparer>
{
    public static ChunkPositionEqualityComparer Instance { get; } = new();

    private ChunkPositionEqualityComparer()
    {
    }

    public bool Equals(ChunkPosition left, ChunkPosition right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public int GetHashCode(ChunkPosition chunkPosition)
    {
        var x = (uint)chunkPosition.X;
        var y = (uint)chunkPosition.Y;

        y = BitOperations.RotateLeft(y, 16);
        return (int)(x ^ y);
    }

    public override bool Equals(object? obj) => obj is ChunkPositionEqualityComparer;
    public bool Equals(ChunkPositionEqualityComparer? other) => other is not null;
    public override int GetHashCode() => nameof(ChunkPositionEqualityComparer).GetHashCode();
}