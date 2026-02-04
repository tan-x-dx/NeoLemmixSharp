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
    private const int BitBuffer32Length = 1;

    private uint _0;

    public readonly int Length => BitBuffer32Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, BitBuffer32Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, BitBuffer32Length);
}

public readonly struct ArrayBitBuffer : IBitBuffer
{
    private readonly uint[] _array;

    public int Length => _array.Length;

    public ArrayBitBuffer(int numberOfItems)
    {
        _array = BitArrayHelpers.CreateBitArray(numberOfItems, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => new(_array);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(_array);
}

public readonly unsafe struct RawBitBuffer : IBitBuffer
{
    private readonly void* _pointer;
    private readonly int _length;

    public int Length => _length;

    public RawBitBuffer(void* pointer, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        _pointer = pointer;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<uint> AsSpan() => Helpers.CreateSpan<uint>(_pointer, _length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ReadOnlySpan<uint> AsReadOnlySpan() => Helpers.CreateReadOnlySpan<uint>(_pointer, _length);
}
