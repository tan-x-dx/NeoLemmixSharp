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

[InlineArray(BitBuffer32Length)]
public struct BitBuffer32 : IBitBuffer
{
    private const int BitBuffer32Length = 32 / 32;

    private uint _0;

    public readonly int Length => BitBuffer32Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, BitBuffer32Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, BitBuffer32Length);
}

[InlineArray(BitBuffer64Length)]
public struct BitBuffer64 : IBitBuffer
{
    private const int BitBuffer64Length = 64 / 32;

    private uint _0;

    public readonly int Length => BitBuffer64Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, BitBuffer64Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, BitBuffer64Length);
}

[InlineArray(BitBuffer96Length)]
public struct BitBuffer96 : IBitBuffer
{
    private const int BitBuffer96Length = 96 / 32;

    private uint _0;

    public readonly int Length => BitBuffer96Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, BitBuffer96Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, BitBuffer96Length);
}

[InlineArray(BitBuffer128Length)]
public struct BitBuffer128 : IBitBuffer
{
    private const int BitBuffer128Length = 128 / 32;

    private uint _0;

    public readonly int Length => BitBuffer128Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, BitBuffer128Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, BitBuffer128Length);
}

[InlineArray(BitBuffer256Length)]
public struct BitBuffer256 : IBitBuffer
{
    private const int BitBuffer256Length = 256 / 32;

    private uint _0;

    public readonly int Length => BitBuffer256Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, BitBuffer256Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, BitBuffer256Length);
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