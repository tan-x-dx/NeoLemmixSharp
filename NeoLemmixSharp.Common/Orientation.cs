using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public readonly struct Orientation : IIdEquatable<Orientation>
{
    public static readonly Orientation Down = new(EngineConstants.DownOrientationRotNum);
    public static readonly Orientation Left = new(EngineConstants.LeftOrientationRotNum);
    public static readonly Orientation Up = new(EngineConstants.UpOrientationRotNum);
    public static readonly Orientation Right = new(EngineConstants.RightOrientationRotNum);

    public readonly int RotNum;

    public Orientation(int rotNum)
    {
        RotNum = rotNum & 3;
    }

    [Pure]
    [DebuggerStepThrough]
    public bool IsParallelTo(Orientation other) => ((RotNum ^ other.RotNum ^ 1) & 1) != 0;
    [Pure]
    [DebuggerStepThrough]
    public bool IsPerpendicularTo(Orientation other) => ((RotNum ^ other.RotNum) & 1) != 0;

    [Pure]
    [DebuggerStepThrough]
    public bool IsVertical() => ((RotNum ^ 1) & 1) != 0;
    [Pure]
    [DebuggerStepThrough]
    public bool IsHorizontal() => (RotNum & 1) != 0;

    [Pure]
    [DebuggerStepThrough]
    public Orientation RotateClockwise() => new(RotNum + 1);
    [Pure]
    [DebuggerStepThrough]
    public Orientation GetOpposite() => new(RotNum + 2);
    [Pure]
    [DebuggerStepThrough]
    public Orientation RotateCounterClockwise() => new(RotNum + 3);
    [Pure]
    [DebuggerStepThrough]
    public Orientation Rotate(int clockwiseRotationOffset) => new(RotNum + clockwiseRotationOffset);

    [Pure]
    [DebuggerStepThrough]
    public float GetRotationAngle()
    {
        ReadOnlySpan<float> RotationAngles =
        [
            EngineConstants.DownOrientationRotationAngle,
            EngineConstants.LeftOrientationRotationAngle,
            EngineConstants.UpOrientationRotationAngle,
            EngineConstants.RightOrientationRotationAngle,
        ];

        return RotationAngles[RotNum & 3];
    }

    [DoesNotReturn]
    public static T ThrowOrientationOutOfRangeException<T>(Orientation orientation)
    {
        throw new ArgumentOutOfRangeException(nameof(RotNum), orientation.RotNum, "Invalid Orientation value!");
    }

    [Pure]
    int IIdEquatable<Orientation>.Id => RotNum;

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(Orientation other) => RotNum == other.RotNum;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Orientation other && Equals(other);
    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode() => RotNum;
    [Pure]
    [DebuggerStepThrough]
    public override string ToString()
    {
        ReadOnlySpan<string> OrientationNames =
        [
            EngineConstants.DownOrientationName,
            EngineConstants.LeftOrientationName,
            EngineConstants.UpOrientationName,
            EngineConstants.RightOrientationName,
        ];

        return OrientationNames[RotNum & 3];
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        var constString = ToString();
        if (destination.Length < constString.Length)
        {
            charsWritten = 0;
            return false;
        }

        constString.AsSpan().CopyTo(destination);
        charsWritten = constString.Length;
        return true;
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(Orientation first, Orientation second) => first.Equals(second.RotNum);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(Orientation first, Orientation second) => !first.Equals(second.RotNum);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static BitArraySet<OrientationHasher, BitBuffer32, Orientation> CreateBitArraySet() => new(new OrientationHasher());
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
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
