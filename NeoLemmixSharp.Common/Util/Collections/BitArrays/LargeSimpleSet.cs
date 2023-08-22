using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class LargeSimpleSet<T> : ISet<T>, IReadOnlySet<T>
{
    private readonly ISimpleHasher<T> _hasher;
    private readonly LargeBitArray _bits;

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
            array[arrayIndex++] = _hasher.Unhash(i);
        }
    }

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<T>
    {
        private readonly ISimpleHasher<T> _hasher;
        private LargeBitArray.Enumerator _bitEnumerator;

        public Enumerator(LargeSimpleSet<T> set)
        {
            _hasher = set._hasher;
            _bitEnumerator = set._bits.GetEnumerator();
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();
        public void Reset() => _bitEnumerator.Reset();
        public readonly T Current => _hasher.Unhash(_bitEnumerator.Current);

        readonly void IDisposable.Dispose() { }
        readonly object IEnumerator.Current => Current!;
    }

    private LargeBitArray BitsFromEnumerable(IEnumerable<T> other)
    {
        if (other is LargeSimpleSet<T> set)
        {
            Debug.Assert(_hasher.Equals(set._hasher));
            return set._bits;
        }

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
        _bits.UnionWith(otherBits);
    }

    public void UnionWith(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        _bits.UnionWith(otherBits);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.IntersectWith(otherBits);
    }

    public void IntersectWith(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        _bits.IntersectWith(otherBits);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.ExceptWith(otherBits);
    }

    public void ExceptWith(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        _bits.ExceptWith(otherBits);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.SymmetricExceptWith(otherBits);
    }

    public void SymmetricExceptWith(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        _bits.SymmetricExceptWith(otherBits);
    }

    [Pure]
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsSubsetOf(otherBits);
    }

    [Pure]
    public bool IsSubsetOf(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        return _bits.IsSubsetOf(otherBits);
    }

    [Pure]
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsSupersetOf(otherBits);
    }

    [Pure]
    public bool IsSupersetOf(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        return _bits.IsSupersetOf(otherBits);
    }

    [Pure]
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsProperSubsetOf(otherBits);
    }

    [Pure]
    public bool IsProperSubsetOf(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        return _bits.IsProperSubsetOf(otherBits);
    }

    [Pure]
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsProperSupersetOf(otherBits);
    }

    [Pure]
    public bool IsProperSupersetOf(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        return _bits.IsProperSupersetOf(otherBits);
    }

    [Pure]
    public bool Overlaps(IEnumerable<T> other)
    {
        if (other is LargeSimpleSet<T> set)
            return _bits.Overlaps(set._bits);

        return other.Any(Contains);
    }

    [Pure]
    public bool Overlaps(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        return _bits.Overlaps(otherBits);
    }

    [Pure]
    public bool SetEquals(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.SetEquals(otherBits);
    }

    [Pure]
    public bool SetEquals(LargeSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        return _bits.SetEquals(otherBits);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;
}