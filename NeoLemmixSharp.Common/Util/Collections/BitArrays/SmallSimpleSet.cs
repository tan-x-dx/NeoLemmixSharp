using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class SmallSimpleSet<T> : ISet<T>, IReadOnlySet<T>
{
    private readonly ISimpleHasher<T> _hasher;
    private readonly SmallBitArray _bits;

    public SmallSimpleSet(ISimpleHasher<T> hasher, bool fullSet = false)
    {
        _hasher = hasher;

        if (_hasher.NumberOfItems > SmallBitArray.Size)
            throw new InvalidOperationException("Small Simple Set can accept at most 32 items!");

        var initialBits = fullSet
            ? (1U << _hasher.NumberOfItems) - 1U
            : 0U;
        _bits = new SmallBitArray(initialBits);
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
        private SmallBitArray.Enumerator _bitEnumerator;

        public Enumerator(SmallSimpleSet<T> set)
        {
            _hasher = set._hasher;
            _bitEnumerator = set._bits.GetEnumerator();
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();
        public void Reset() => _bitEnumerator.Reset();
        public T Current => _hasher.Unhash(_bitEnumerator.Current);

        void IDisposable.Dispose() { }
        object IEnumerator.Current => Current!;
    }

    private uint BitsFromEnumerable(IEnumerable<T> other)
    {
        if (other is SmallSimpleSet<T> set)
        {
            Debug.Assert(_hasher.Equals(set._hasher));
            return set._bits.GetRawBits();
        }

        var result = 0U;

        foreach (var item in other)
        {
            var index = _hasher.Hash(item);
            result |= 1U << index;
        }

        return result;
    }

    public void UnionWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.UnionWith(otherBits);
    }

    public void UnionWith(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits.GetRawBits();
        _bits.UnionWith(otherBits);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.IntersectWith(otherBits);
    }

    public void IntersectWith(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits.GetRawBits();
        _bits.IntersectWith(otherBits);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.ExceptWith(otherBits);
    }

    public void ExceptWith(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits.GetRawBits();
        _bits.ExceptWith(otherBits);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.SymmetricExceptWith(otherBits);
    }

    public void SymmetricExceptWith(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits.GetRawBits();
        _bits.SymmetricExceptWith(otherBits);
    }

    [Pure]
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsSubsetOf(otherBits);
    }

    [Pure]
    public bool IsSubsetOf(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits.GetRawBits();
        return _bits.IsSubsetOf(otherBits);
    }

    [Pure]
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsSupersetOf(otherBits);
    }

    [Pure]
    public bool IsSupersetOf(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits.GetRawBits();
        return _bits.IsSupersetOf(otherBits);
    }

    [Pure]
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsProperSubsetOf(otherBits);
    }

    [Pure]
    public bool IsProperSubsetOf(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits.GetRawBits();
        return _bits.IsProperSubsetOf(otherBits);
    }

    [Pure]
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsProperSupersetOf(otherBits);
    }

    [Pure]
    public bool IsProperSupersetOf(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits.GetRawBits();
        return _bits.IsProperSupersetOf(otherBits);
    }

    [Pure]
    public bool Overlaps(IEnumerable<T> other)
    {
        if (other is SmallSimpleSet<T> set)
            return _bits.Overlaps(set._bits.GetRawBits());

        return other.Any(Contains);
    }

    [Pure]
    public bool Overlaps(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        return _bits.Overlaps(otherBits.GetRawBits());
    }

    [Pure]
    public bool SetEquals(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.SetEquals(otherBits);
    }

    [Pure]
    public bool SetEquals(SmallSimpleSet<T> other)
    {
        Debug.Assert(_hasher.Equals(other._hasher));
        var otherBits = other._bits;
        return _bits.SetEquals(otherBits.GetRawBits());
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;
}