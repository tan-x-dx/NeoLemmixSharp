using System.Diagnostics;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal readonly struct ChunkPosition : IEquatable<ChunkPosition>
{
    public readonly byte X;
    public readonly byte Y;

    public ChunkPosition(int x, int y)
    {
        Debug.Assert(x >= 0 && x < 256 &&
                     y >= 0 && y < 256);

        X = (byte)x;
        Y = (byte)y;
    }

    public override string ToString() => $"[{X}, {Y}]";

    public override bool Equals(object? obj) => obj is ChunkPosition other && X == other.X && Y == other.Y;
    public bool Equals(ChunkPosition other) => X == other.X && Y == other.Y;
    public override int GetHashCode()
    {
        return X | (Y << 16);
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
        return chunkPosition.X | (chunkPosition.Y << 16);
    }

    public override bool Equals(object? obj) => obj is ChunkPositionEqualityComparer;
    public bool Equals(ChunkPositionEqualityComparer? other) => other is not null;
    public override int GetHashCode() => nameof(ChunkPositionEqualityComparer).GetHashCode();
}