using NeoLemmixSharp.Common.Util.Collections;

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

    public SimpleSet<T>.Enumerator GetEnumerator()
    {
        return new SimpleSet<T>.Enumerator(_hasher, _bits, _count);
    }
}