using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitBuffers;

[InlineArray(Length)]
public struct BitBuffer32 : ISpannable
{
    private const int Length = 1;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

[InlineArray(Length)]
public struct BitBuffer64 : ISpannable
{
    private const int Length = 2;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

[InlineArray(Length)]
public struct BitBuffer96 : ISpannable
{
    private const int Length = 3;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

[InlineArray(Length)]
public struct BitBuffer128 : ISpannable
{
    private const int Length = 4;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

[InlineArray(Length)]
public struct BitBuffer256 : ISpannable
{
    private const int Length = 8;

    private uint _0;

    public readonly int Size => Length;

    public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, Length);
    public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, Length);
}

public readonly struct ArrayWrapper : ISpannable
{
    private readonly uint[] _array;
    private readonly int _start;
    private readonly int _length;

    public int Size => _length;

    public ArrayWrapper(uint[] array)
    {
        _array = array;
        _start = 0;
        _length = _array.Length;
    }

    public ArrayWrapper(uint[] array, int start, int length)
    {
        _array = array;
        _start = start;
        _length = length;
    }

    public Span<uint> AsSpan() => new(_array, _start, _length);
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(_array, _start, _length);
}