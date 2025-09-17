using System.Numerics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public ref struct BitArrayEnumerator<TPerfectHasher, T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    private readonly TPerfectHasher _hasher;
    private readonly ReadOnlySpan<uint> _bits;

    private int _remaining;
    private int _index;
    private int _current;
    private uint _v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal BitArrayEnumerator(
        TPerfectHasher hasher,
        ReadOnlySpan<uint> bits,
        int count)
    {
        _hasher = hasher;
        _bits = bits;
        _remaining = count;
        _index = 0;
        _current = 0;
        _v = _bits.Length == 0 ? 0U : _bits[0];
    }

    public bool MoveNext()
    {
        if (_v == 0U)
        {
            if (_remaining == 0)
                return false;

            do
            {
                _v = _bits[++_index];
            }
            while (_v == 0U);
        }

        _current = (_index << BitArrayHelpers.Shift) | BitOperations.TrailingZeroCount(_v);
        _v &= _v - 1;
        _remaining--;
        return true;
    }

    public readonly T Current => _hasher.UnHash(_current);
}

public ref struct BitArrayEnumerator
{
    private readonly ReadOnlySpan<uint> _bits;

    private int _remaining;
    private int _index;
    private int _current;
    private uint _v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayEnumerator(
        ReadOnlySpan<uint> bits,
        int count)
    {
        _bits = bits;
        _remaining = count;
        _index = 0;
        _current = 0;
        _v = _bits.Length == 0 ? 0U : _bits[0];
    }

    public bool MoveNext()
    {
        if (_v == 0U)
        {
            if (_remaining == 0)
                return false;

            do
            {
                _v = _bits[++_index];
            }
            while (_v == 0U);
        }

        _current = (_index << BitArrayHelpers.Shift) | BitOperations.TrailingZeroCount(_v);
        _v &= _v - 1;
        _remaining--;
        return true;
    }

    public readonly int Current => _current;
}
