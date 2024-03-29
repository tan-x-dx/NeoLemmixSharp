﻿using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using BitArray = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArray;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class SimpleSet<T> : ISet<T>, IReadOnlySet<T>
{
    private readonly IPerfectHasher<T> _hasher;
    private readonly BitArray _bits;

    public SimpleSet(IPerfectHasher<T> hasher, bool fullSet = false)
    {
        _hasher = hasher;
        _bits = new BitArray(hasher.NumberOfItems, fullSet);
    }

    public static SimpleSet<T> CreateFullSet(IPerfectHasher<T> hasher)
    {
        return new SimpleSet<T>(hasher, true);
    }

    public int Size => _bits.Size;
    public int Count => _bits.PopCount;

    public bool Add(T item)
    {
        var hash = _hasher.Hash(item);
        return _bits.SetBit(hash);
    }

    void ICollection<T>.Add(T item) => Add(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    public T[] ToArray()
    {
        var result = new T[_bits.PopCount];
        CopyTo(result, 0);
        return result;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        var iterator = new BitBasedEnumerator<T>(_hasher, _bits.AsReadOnlySpan(), _bits.PopCount);
        while (iterator.MoveNext())
        {
            array[arrayIndex++] = iterator.Current;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleSetEnumerable<T> ToSimpleEnumerable() => new(_hasher, _bits.AsReadOnlySpan(), _bits.PopCount);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBasedEnumerator<T> GetEnumerator() => new(_hasher, _bits.AsReadOnlySpan(), _bits.PopCount);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReferenceTypeEnumerator GetReferenceTypeEnumerator() => new(this);
    [Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ReferenceTypeEnumerator(this);
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => new ReferenceTypeEnumerator(this);

    public sealed class ReferenceTypeEnumerator : IEnumerator<T>
    {
        private readonly IPerfectHasher<T> _hasher;
        private readonly BitArray.ReferenceTypeBitEnumerator _bitEnumerator;

        public ReferenceTypeEnumerator(SimpleSet<T> set)
        {
            _hasher = set._hasher;
            _bitEnumerator = set._bits.GetEnumerator();
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();
        public void Reset() => _bitEnumerator.Reset();
        public T Current => _hasher.UnHash(_bitEnumerator.Current);

        void IDisposable.Dispose() { }
        object IEnumerator.Current => Current!;
    }

    [Pure]
    private BitArray BitsFromEnumerable(IEnumerable<T> other)
    {
        if (other is SimpleSet<T> set)
            return set._bits;

        var result = new BitArray(_hasher.NumberOfItems);

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
        _bits.UnionWith(otherBits.AsReadOnlySpan());
    }

    public void UnionWith(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        _bits.UnionWith(otherBits.AsReadOnlySpan());
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.IntersectWith(otherBits.AsReadOnlySpan());
    }

    public void IntersectWith(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        _bits.IntersectWith(otherBits.AsReadOnlySpan());
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.ExceptWith(otherBits.AsReadOnlySpan());
    }

    public void ExceptWith(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        _bits.ExceptWith(otherBits.AsReadOnlySpan());
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        _bits.SymmetricExceptWith(otherBits.AsReadOnlySpan());
    }

    public void SymmetricExceptWith(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        _bits.SymmetricExceptWith(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsSubsetOf(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsSubsetOf(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.IsSubsetOf(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsSupersetOf(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsSupersetOf(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.IsSupersetOf(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsProperSubsetOf(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsProperSubsetOf(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.IsProperSubsetOf(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.IsProperSupersetOf(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsProperSupersetOf(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.IsProperSupersetOf(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool Overlaps(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.Overlaps(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool Overlaps(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.Overlaps(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool SetEquals(IEnumerable<T> other)
    {
        var otherBits = BitsFromEnumerable(other);
        return _bits.SetEquals(otherBits.AsReadOnlySpan());
    }

    [Pure]
    public bool SetEquals(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return _bits.SetEquals(otherBits.AsReadOnlySpan());
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;
}