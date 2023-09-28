using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class SingleUintWrapper : IUintWrapper
{
    public const int SmallBitArraySize = 1;
    public const int SmallBitArrayLength = SmallBitArraySize << BitArray.Shift;

    private uint _bits;

    public SingleUintWrapper(uint initialBits = 0U)
    {
        _bits = initialBits;
    }

    public int Size => SmallBitArraySize;

    public void Clear() => _bits = 0U;

    [Pure]
    public Span<uint> AsSpan() => new(ref _bits);

    [Pure]
    public ReadOnlySpan<uint> AsReadOnlySpan() => new(in _bits);
}