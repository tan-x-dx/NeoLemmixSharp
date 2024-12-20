﻿using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class SimpleSet<TPerfectHasher, T> : ISet<T>, IReadOnlySet<T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    private const int MaxStackAllocSize = 64;

    private readonly TPerfectHasher _hasher;
    private readonly uint[] _bits;
    private int _popCount;

    public SimpleSet(TPerfectHasher hasher, bool fullSet)
    {
        _hasher = hasher;
        var numberOfItems = hasher.NumberOfItems;
        _bits = BitArrayHelpers.CreateBitArray(numberOfItems, fullSet);
        _popCount = fullSet
            ? numberOfItems
            : 0;
    }

    public int Count => _popCount;

    public bool Add(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArrayHelpers.SetBit(new Span<uint>(_bits), hash, ref _popCount);
    }

    void ICollection<T>.Add(T item) => Add(item);

    public void Clear()
    {
        new Span<uint>(_bits).Clear();
        _popCount = 0;
    }

    [Pure]
    public bool Contains(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_bits), hash);
    }

    public bool Remove(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArrayHelpers.ClearBit(new Span<uint>(_bits), hash, ref _popCount);
    }

    /// <summary>
    /// Updates the contents of this collection to be identical to the contents of the other collection.
    /// </summary>
    /// <param name="other">The set whose values will be copied into this one.</param>
    public void SetFrom(SimpleSet<TPerfectHasher, T> other)
    {
        new ReadOnlySpan<uint>(other._bits).CopyTo(new Span<uint>(_bits));
        _popCount = other._popCount;
    }

    [Pure]
    public T[] ToArray()
    {
        var count = _popCount;

        if (count == 0)
            return Array.Empty<T>();

        var result = new T[count];
        CopyTo(result, 0);
        return result;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        var iterator = new BitBasedEnumerator<TPerfectHasher, T>(_hasher, new ReadOnlySpan<uint>(_bits), _popCount);
        while (iterator.MoveNext())
        {
            array[arrayIndex++] = iterator.Current;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleSetEnumerable<TPerfectHasher, T> AsSimpleEnumerable() => new(_hasher, new ReadOnlySpan<uint>(_bits), _popCount);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBasedEnumerator<TPerfectHasher, T> GetEnumerator() => new(_hasher, new ReadOnlySpan<uint>(_bits), _popCount);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReferenceTypeEnumerator GetReferenceTypeEnumerator() => new(this);
    [Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ReferenceTypeEnumerator(this);
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => new ReferenceTypeEnumerator(this);

    public sealed class ReferenceTypeEnumerator : IEnumerator<T>
    {
        private readonly TPerfectHasher _hasher;
        private readonly BitArrayHelpers.ReferenceTypeBitEnumerator _bitEnumerator;

        public ReferenceTypeEnumerator(SimpleSet<TPerfectHasher, T> set)
        {
            _hasher = set._hasher;
            _bitEnumerator = new BitArrayHelpers.ReferenceTypeBitEnumerator(set._bits, set._popCount);
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();
        public T Current => _hasher.UnHash(_bitEnumerator.Current);

        void IEnumerator.Reset() => throw new InvalidOperationException("Cannot reset");
        void IDisposable.Dispose() { }
        object IEnumerator.Current => Current;
    }

    private void GetBitsFromEnumerable(Span<uint> buffer, IEnumerable<T> other)
    {
        if (other is SimpleSet<TPerfectHasher, T> set)
        {
            new ReadOnlySpan<uint>(set._bits).CopyTo(buffer);
            return;
        }

        foreach (var item in other)
        {
            var index = _hasher.Hash(item);
            BitArrayHelpers.SetBit(buffer, index);
        }
    }

    public void UnionWith(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArrayHelpers.UnionWith(new Span<uint>(_bits), otherBitBuffer);
        _popCount = BitArrayHelpers.GetPopCount(new ReadOnlySpan<uint>(_bits));
    }

    public void UnionWith(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        BitArrayHelpers.UnionWith(new Span<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
        _popCount = BitArrayHelpers.GetPopCount(new ReadOnlySpan<uint>(_bits));
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArrayHelpers.IntersectWith(new Span<uint>(_bits), otherBitBuffer);
        _popCount = BitArrayHelpers.GetPopCount(new ReadOnlySpan<uint>(_bits));
    }

    public void IntersectWith(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        BitArrayHelpers.IntersectWith(new Span<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
        _popCount = BitArrayHelpers.GetPopCount(new ReadOnlySpan<uint>(_bits));
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArrayHelpers.ExceptWith(new Span<uint>(_bits), otherBitBuffer);
        _popCount = BitArrayHelpers.GetPopCount(new ReadOnlySpan<uint>(_bits));
    }

    public void ExceptWith(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        BitArrayHelpers.ExceptWith(new Span<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
        _popCount = BitArrayHelpers.GetPopCount(new ReadOnlySpan<uint>(_bits));
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArrayHelpers.SymmetricExceptWith(new Span<uint>(_bits), otherBitBuffer);
        _popCount = BitArrayHelpers.GetPopCount(new ReadOnlySpan<uint>(_bits));
    }

    public void SymmetricExceptWith(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        BitArrayHelpers.SymmetricExceptWith(new Span<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
        _popCount = BitArrayHelpers.GetPopCount(new ReadOnlySpan<uint>(_bits));
    }

    [Pure]
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.IsSubsetOf(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool IsSubsetOf(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        return BitArrayHelpers.IsSubsetOf(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.IsSupersetOf(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool IsSupersetOf(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        return BitArrayHelpers.IsSupersetOf(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.IsProperSubsetOf(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool IsProperSubsetOf(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        return BitArrayHelpers.IsProperSubsetOf(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.IsProperSupersetOf(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool IsProperSupersetOf(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        return BitArrayHelpers.IsProperSupersetOf(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool Overlaps(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.Overlaps(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool Overlaps(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        return BitArrayHelpers.Overlaps(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool SetEquals(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.SetEquals(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool SetEquals(SimpleSet<TPerfectHasher, T> other)
    {
        var otherBits = other._bits;
        return BitArrayHelpers.SetEquals(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;
}