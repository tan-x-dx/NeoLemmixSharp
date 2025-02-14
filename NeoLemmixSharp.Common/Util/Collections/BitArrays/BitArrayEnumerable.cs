using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public readonly ref struct BitArrayEnumerable<TPerfectHasher, T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    public static BitArrayEnumerable<TPerfectHasher, T> Empty => default;

    private readonly TPerfectHasher _hasher;
    private readonly ReadOnlySpan<uint> _bits;
    public readonly int Count;

    internal BitArrayEnumerable(TPerfectHasher hasher, ReadOnlySpan<uint> bits, int count)
    {
        _hasher = hasher;
        _bits = bits;
        Count = count;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayEnumerator<TPerfectHasher, T> GetEnumerator() => new(_hasher, _bits, Count);
}