using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface IBitBuffer
{
    [Pure]
    int Length { get; }

    [Pure]
    Span<uint> AsSpan();
    [Pure]
    ReadOnlySpan<uint> AsReadOnlySpan();
}

public interface IBitBufferCreator<TBuffer>
    where TBuffer : struct, IBitBuffer
{
    void CreateBitBuffer(out TBuffer buffer);
}

public struct BitBuffer32 : IBitBuffer
{
    private uint _0;

    public readonly int Length => 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, 1);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, 1);
}

public readonly struct ArrayBitBuffer : IBitBuffer
{
    private readonly int _start;
    private readonly int _length;
    private readonly uint[] _array;

    public int Length => _length;

    public ArrayBitBuffer(uint[] array)
    {
        _array = array;
        _start = 0;
        _length = _array.Length;
    }

    public ArrayBitBuffer(int numberOfItems)
    {
        _array = BitArrayHelpers.CreateBitArray(numberOfItems, false);
        _start = 0;
        _length = _array.Length;
    }

    public ArrayBitBuffer(uint[] array, int start, int length)
    {
        _array = array;

        // Shamelessly stolen from the dotNet source for spans...
#if TARGET_64BIT
            // Since start and length are both 32-bit, their sum can be computed across a 64-bit domain
            // without loss of fidelity. The cast to uint before the cast to ulong ensures that the
            // extension from 32- to 64-bit is zero-extending rather than sign-extending. The end result
            // of this is that if either input is negative or if the input sum overflows past Int32.MaxValue,
            // that information is captured correctly in the comparison against the backing _length field.
            // We don't use this same mechanism in a 32-bit process due to the overhead of 64-bit arithmetic.
            if ((ulong)(uint)start + (ulong)(uint)length > (ulong)(uint)array.Length)
                ThrowArgumentOutOfRangeException();
#else
        if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
            ThrowArgumentOutOfRangeException();
#endif

        _start = start;
        _length = length;
    }

    [DoesNotReturn]
    private static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => new(_array, _start, _length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(_array, _start, _length);
}