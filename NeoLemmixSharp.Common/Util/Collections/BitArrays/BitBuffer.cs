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
        _start = start;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => new(_array, _start, _length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(_array, _start, _length);
}

public unsafe readonly struct RawBitBuffer : IBitBuffer
{
    private readonly void* _pointer;
    private readonly int _length;

    public int Length => _length;

    public RawBitBuffer(void* pointer, int length)
    {
        _pointer = pointer;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => new(_pointer, _length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(_pointer, _length);
}
