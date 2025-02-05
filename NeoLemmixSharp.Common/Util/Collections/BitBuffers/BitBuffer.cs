using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitBuffers;

[InlineArray(Length)]
public struct BitBuffer32 : IBitBuffer
{
    private const int Length = 1;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

[InlineArray(Length)]
public struct BitBuffer64 : IBitBuffer
{
    private const int Length = 2;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

[InlineArray(Length)]
public struct BitBuffer96 : IBitBuffer
{
    private const int Length = 3;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

[InlineArray(Length)]
public struct BitBuffer128 : IBitBuffer
{
    private const int Length = 4;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

[InlineArray(Length)]
public struct BitBuffer256 : IBitBuffer
{
    private const int Length = 8;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

public readonly struct ArrayBitBuffer : IBitBuffer
{
    private readonly uint[] _array;
    private readonly int _start;
    private readonly int _length;

    public int Size => _length;

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

    public Span<uint> AsSpan() => new(_array, _start, _length);
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(_array, _start, _length);
}