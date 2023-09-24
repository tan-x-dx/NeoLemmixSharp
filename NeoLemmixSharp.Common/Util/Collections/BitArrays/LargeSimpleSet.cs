using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class LargeSimpleSet<T> : ISet<T>, IReadOnlySet<T>
{
    public static LargeSimpleSet<T> Empty { get; } = new();

    private readonly ISimpleHasher<T> _hasher;
    private readonly LargeBitArray _bits;

    private LargeSimpleSet()
    {
        _hasher = ISimpleHasher<T>.Empty;
        _bits = LargeBitArray.Empty;
    }

    public LargeSimpleSet(ISimpleHasher<T> hasher, bool fullSet = false)
    {
        _hasher = hasher;
        _bits = new LargeBitArray(_hasher.NumberOfItems, fullSet);
    }

    public int Count => _bits.Count;

    public bool Add(T item)
    {
        var hash = _hasher.Hash(item);
        return _bits.SetBit(hash);
    }

    void ICollection<T>.Add(T item) => Add(item);

    public void Clear() => _bits.Clear();

    [Pure]
    public bool Contains(T item)
    {
        var hash = _hasher.Hash(item);
        return _bits.GetBit(hash);
    }

    public bool Remove(T item)
    {
        var hash = _hasher.Hash(item);
        return _bits.ClearBit(hash);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var i in _bits)
        {
            array[arrayIndex++] = _hasher.UnHash(i);
        }
    }

    [Pure]
    public Enumerator GetEnumerator() => new(this);
    [Pure]
    public ReferenceTypeEnumerator GetReferenceTypeEnumerator() => new(this);
    [Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ReferenceTypeEnumerator(this);
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => new ReferenceTypeEnumerator(this);

    public ref struct Enumerator
    {
        private readonly ISimpleHasher<T> _hasher;
        private LargeBitArray.Enumerator _bitEnumerator;

        public Enumerator(LargeSimpleSet<T> set)
        {
            _hasher = set._hasher;
            _bitEnumerator = set._bits.GetEnumerator();
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();
        public readonly T Current => _hasher.UnHash(_bitEnumerator.Current);
    }

    public sealed class ReferenceTypeEnumerator : IEnumerator<T>
    {
        private readonly ISimpleHasher<T> _hasher;
        private readonly LargeBitArray.ReferenceTypeEnumerator _bitEnumerator;

        public ReferenceTypeEnumerator(LargeSimpleSet<T> set)
        {
            _hasher = set._hasher;
            _bitEnumerator = set._bits.GetReferenceTypeEnumerator();
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();
        public void Reset() => _bitEnumerator.Reset();
        public T Current => _hasher.UnHash(_bitEnumerator.Current);

        void IDisposable.Dispose() { }
        object IEnumerator.Current => Current!;
    }

    private LargeBitArray BitsFromEnumerable(IEnumerable<T> other)
    {
        if (other is LargeSimpleSet<T> set)
            return set._bits;

        var result = new LargeBitArray(_hasher.NumberOfItems);

        foreach (var item in other)
        {
            var index = _hasher.Hash(item);
            result.SetBit(index);
        }

        return result;
    }

    public void UnionWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.UnionWith(otherBits.AsSpan());
    }

    public void UnionWith(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        _bits.UnionWith(otherBits.AsSpan());
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.IntersectWith(otherBits.AsSpan());
    }

    public void IntersectWith(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        _bits.IntersectWith(otherBits.AsSpan());
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.ExceptWith(otherBits.AsSpan());
    }

    public void ExceptWith(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        _bits.ExceptWith(otherBits.AsSpan());
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.SymmetricExceptWith(otherBits.AsSpan());
    }

    public void SymmetricExceptWith(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        _bits.SymmetricExceptWith(otherBits.AsSpan());
    }

    [Pure]
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsSubsetOf(otherBits.AsSpan());
    }

    [Pure]
    public bool IsSubsetOf(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.IsSubsetOf(otherBits.AsSpan());
    }

    [Pure]
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsSupersetOf(otherBits.AsSpan());
    }

    [Pure]
    public bool IsSupersetOf(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.IsSupersetOf(otherBits.AsSpan());
    }

    [Pure]
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsProperSubsetOf(otherBits.AsSpan());
    }

    [Pure]
    public bool IsProperSubsetOf(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.IsProperSubsetOf(otherBits.AsSpan());
    }

    [Pure]
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsProperSupersetOf(otherBits.AsSpan());
    }

    [Pure]
    public bool IsProperSupersetOf(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.IsProperSupersetOf(otherBits.AsSpan());
    }

    [Pure]
    public bool Overlaps(IEnumerable<T> other)
    {
        if (other is LargeSimpleSet<T> set)
            return _bits.Overlaps(set._bits.AsSpan());

        return other.Any(Contains);
    }

    [Pure]
    public bool Overlaps(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.Overlaps(otherBits.AsSpan());
    }

    [Pure]
    public bool SetEquals(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.SetEquals(otherBits.AsSpan());
    }

    [Pure]
    public bool SetEquals(LargeSimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.SetEquals(otherBits.AsSpan());
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;
}