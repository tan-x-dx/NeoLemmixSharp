using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

public readonly struct Orientation : IIdEquatable<Orientation>
{
    public static readonly Orientation Down = new(EngineConstants.DownOrientationRotNum);
    public static readonly Orientation Left = new(EngineConstants.LeftOrientationRotNum);
    public static readonly Orientation Up = new(EngineConstants.UpOrientationRotNum);
    public static readonly Orientation Right = new(EngineConstants.RightOrientationRotNum);

    private static ReadOnlySpan<int> RawInts =>
    [
        EngineConstants.DownOrientationRotNum,
        EngineConstants.LeftOrientationRotNum,
        EngineConstants.UpOrientationRotNum,
        EngineConstants.RightOrientationRotNum
    ];
    public static ReadOnlySpan<Orientation> AllItems => MemoryMarshal.Cast<int, Orientation>(RawInts);

    public readonly int RotNum;

    public Orientation(int rotNum)
    {
        RotNum = rotNum & 3;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsParallelTo(Orientation other) => ((RotNum ^ other.RotNum ^ 1) & 1) != 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsPerpendicularTo(Orientation other) => ((RotNum ^ other.RotNum) & 1) != 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Orientation RotateClockwise() => new(RotNum + 1);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Orientation GetOpposite() => new(RotNum + 2);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Orientation RotateCounterClockwise() => new(RotNum + 3);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Orientation Rotate(int clockwiseRotationOffset) => new(RotNum + clockwiseRotationOffset);

    [DoesNotReturn]
    public static T ThrowOrientationOutOfRangeException<T>(Orientation orientation)
    {
        throw new ArgumentOutOfRangeException(nameof(RotNum), orientation.RotNum, "Invalid Orientation value!");
    }

    int IIdEquatable<Orientation>.Id => RotNum;

    public bool Equals(Orientation other) => RotNum == other.RotNum;
    public override bool Equals(object? obj) => obj is Orientation other && RotNum == other.RotNum;
    public override int GetHashCode() => RotNum;
    public override string ToString() => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => EngineConstants.DownOrientationName,
        EngineConstants.LeftOrientationRotNum => EngineConstants.LeftOrientationName,
        EngineConstants.UpOrientationRotNum => EngineConstants.UpOrientationName,
        EngineConstants.RightOrientationRotNum => EngineConstants.RightOrientationName,

        _ => ThrowOrientationOutOfRangeException<string>(this)
    };

    public static bool operator ==(Orientation first, Orientation second) => first.RotNum == second.RotNum;
    public static bool operator !=(Orientation first, Orientation second) => first.RotNum != second.RotNum;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<OrientationHasher, BitBuffer32, Orientation> CreateBitArraySet(bool fullSet = false) => new(new OrientationHasher(), fullSet);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<OrientationHasher, BitBuffer32, Orientation, TValue> CreateBitArrayDictionary<TValue>() => new(new OrientationHasher());

    public readonly struct OrientationHasher : IPerfectHasher<Orientation>, IBitBufferCreator<BitBuffer32>
    {
        public int NumberOfItems => EngineConstants.NumberOfOrientations;

        [Pure]
        public int Hash(Orientation item) => item.RotNum;
        [Pure]
        public Orientation UnHash(int index) => new(index);

        public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
    }
}
