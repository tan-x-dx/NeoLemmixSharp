using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public readonly ref struct SimpleSetEnumerable<T>
{
    public static SimpleSetEnumerable<T> Empty => default;

    private readonly IPerfectHasher<T> _hasher;
    private readonly ReadOnlySpan<uint> _bits;
    public readonly int Count;

    public SimpleSetEnumerable(IPerfectHasher<T> hasher, ReadOnlySpan<uint> bits, int count)
    {
        _hasher = hasher;
        _bits = bits;
        Count = count;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBasedEnumerator<T> GetEnumerator() => new(_hasher, _bits, Count);
}