using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public readonly ref struct SimpleSetEnumerable<T>
{
    private readonly IPerfectHasher<T> _hasher;
    private readonly ReadOnlySpan<uint> _bits;
    private readonly int _count;

    public static SimpleSetEnumerable<T> Empty => new(null!, ReadOnlySpan<uint>.Empty, 0);

    public bool IsEmpty => _count == 0;

    public SimpleSetEnumerable(IPerfectHasher<T> hasher, ReadOnlySpan<uint> bits, int count)
    {
        _hasher = hasher;
        _bits = bits;
        _count = count;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBasedEnumerator<T> GetEnumerator()
    {
        return new BitBasedEnumerator<T>(_hasher, _bits, _count);
    }
}