using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.Collections;

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
    private readonly uint[] _array;
    private readonly int _start;
    private readonly int _length;

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
        _start = start;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<uint> AsSpan() => new(_array, _start, _length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(_array, _start, _length);
}