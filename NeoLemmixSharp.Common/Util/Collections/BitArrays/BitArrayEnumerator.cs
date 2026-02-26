using System.Numerics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public ref struct BitArrayEnumerator<TPerfectHasher, T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    private readonly ReadOnlySpan<uint> _bits;

    private uint _v;
    private int _index;
    private int _current;

    private readonly TPerfectHasher _hasher;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal BitArrayEnumerator(
        TPerfectHasher hasher,
        ReadOnlySpan<uint> bits)
    {
        _bits = bits;
        _v = bits.Length == 0 ? 0U : bits.At(0);
        _index = 0;
        _current = 0;
        _hasher = hasher;
    }

    public bool MoveNext()
    {
        var v = _v;
        var index = _index;
        if (v == 0U)
        {
            do
            {
                ++index;
                if ((uint)index >= (uint)_bits.Length)
                    return false;

                v = _bits.At(index);
            }
            while (v == 0U);
            _index = index;
        }

        _current = (index << BitArrayHelpers.Shift) | BitOperations.TrailingZeroCount(v);
        v &= v - 1;
        _v = v;
        return true;
    }

    public readonly T Current => _hasher.UnHash(_current);
}

public ref struct BitArrayEnumerator
{
    private readonly ReadOnlySpan<uint> _bits;

    private uint _v;
    private int _index;
    private int _current;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayEnumerator(ReadOnlySpan<uint> bits)
    {
        _bits = bits;
        _v = bits.Length == 0 ? 0U : bits.At(0);
        _index = 0;
        _current = 0;
    }

    public bool MoveNext()
    {
        var v = _v;
        var index = _index;
        if (v == 0U)
        {
            do
            {
                ++index;
                if ((uint)index >= (uint)_bits.Length)
                    return false;

                v = _bits.At(index);
            }
            while (v == 0U);
            _index = index;
        }

        _current = (index << BitArrayHelpers.Shift) | BitOperations.TrailingZeroCount(v);
        v &= v - 1;
        _v = v;
        return true;
    }

    public readonly int Current => _current;
}
