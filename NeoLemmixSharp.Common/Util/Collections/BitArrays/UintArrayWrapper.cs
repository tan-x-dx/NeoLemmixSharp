using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class UintArrayWrapper : IUintWrapper
{
    private const int Mask = (1 << BitArray.Shift) - 1;

    private readonly uint[] _bits;
    private readonly int _offset;

    public int Size { get; }

    public UintArrayWrapper(int length, bool initialBitFlag = false)
    {
        Size = (length + Mask) >> BitArray.Shift;
        _offset = 0;
        _bits = new uint[Size];

        if (!initialBitFlag)
            return;

        Array.Fill(_bits, uint.MaxValue);

        var shift = length & Mask;
        var mask = (1U << shift) - 1U;
        _bits[^1] = mask;
    }

    public UintArrayWrapper(uint[] array, int offset, int length)
    {
        _bits = array;
        _offset = offset;
        Size = length;
    }

    public UintArrayWrapper(uint[] array)
    {
        _bits = array;
        _offset = 0;
        Size = array.Length;
    }

    public void Clear()
    {
        Array.Clear(_bits, _offset, Size);
    }

    [Pure]
    public Span<uint> AsSpan() => new(_bits, _offset, Size);

    [Pure]
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(_bits, _offset, Size);
}