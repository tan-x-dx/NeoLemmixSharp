using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public readonly ref struct SimpleSetEnumerable<TPerfectHasher, T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    public static SimpleSetEnumerable<TPerfectHasher, T> Empty => default;

    private readonly TPerfectHasher _hasher;
    private readonly ReadOnlySpan<uint> _bits;
    public readonly int Count;

    internal SimpleSetEnumerable(TPerfectHasher hasher, ReadOnlySpan<uint> bits, int count)
    {
        _hasher = hasher;
        _bits = bits;
        Count = count;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBasedEnumerator<TPerfectHasher, T> GetEnumerator() => new(_hasher, _bits, Count);
}