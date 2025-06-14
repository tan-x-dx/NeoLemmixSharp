﻿using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class BitArraySet<TPerfectHasher, TBuffer, T> : ISet<T>, IReadOnlySet<T>
    where TPerfectHasher : IPerfectHasher<T>, IBitBufferCreator<TBuffer>
    where TBuffer : struct, IBitBuffer
    where T : notnull
{
    private const int MaxStackAllocSize = 64;

#pragma warning disable IDE0044 // Add readonly modifier
    private TBuffer _bits;
#pragma warning restore IDE0044 // Add readonly modifier
    private int _popCount;
    private readonly TPerfectHasher _hasher;

    public BitArraySet(TPerfectHasher hasher)
    {
        _hasher = hasher;
        _hasher.CreateBitBuffer(out _bits);
        var numberOfItems = _hasher.NumberOfItems;

        BitArrayHelpers.ThrowIfInvalidCapacity(numberOfItems, _bits.Length);

        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    public int Length => _bits.Length;
    public int Count => _popCount;

    public bool Add(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArrayHelpers.SetBit(_bits.AsSpan(), hash, ref _popCount);
    }

    void ICollection<T>.Add(T item) => Add(item);

    public void Clear()
    {
        _bits.AsSpan().Clear();
        _popCount = 0;
    }

    [Pure]
    public bool Contains(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArrayHelpers.GetBit(_bits.AsReadOnlySpan(), hash);
    }

    public bool Remove(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArrayHelpers.ClearBit(_bits.AsSpan(), hash, ref _popCount);
    }

    /// <summary>
    /// Updates the contents of this collection to be identical to the contents of the other collection.
    /// </summary>
    /// <param name="other">The set whose values will be copied into this one.</param>
    public void SetFrom(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        other._bits.AsReadOnlySpan().CopyTo(_bits.AsSpan());
        _popCount = other._popCount;
    }

    public void WriteTo(Span<uint> destination)
    {
        var sourceSpan = _bits.AsReadOnlySpan();
        if (destination.Length < sourceSpan.Length)
            throw new ArgumentException("Destination buffer too small!");
        sourceSpan.CopyTo(destination);
    }

    public void ReadFrom(ReadOnlySpan<uint> source)
    {
        var destSpan = _bits.AsSpan();
        if (source.Length > destSpan.Length)
            throw new ArgumentException("Source buffer too big!");

        if (source.Length == destSpan.Length)
        {
            var upperIntNumberOfItems = _hasher.NumberOfItems & BitArrayHelpers.Mask;

            if (upperIntNumberOfItems != 0)
            {
                var lastInt = source[^1];
                var i = (1U << upperIntNumberOfItems) - 1U;
                if ((lastInt & ~i) != 0U)
                    throw new ArgumentException("Upper bits set outside of valid range");
            }
        }

        destSpan.Clear();
        source.CopyTo(destSpan);
        _popCount = BitArrayHelpers.GetPopCount(source);
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
        var iterator = new BitArrayEnumerator(_bits.AsReadOnlySpan(), _popCount);
        while (iterator.MoveNext())
        {
            array[arrayIndex++] = _hasher.UnHash(iterator.Current);
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayEnumerable<TPerfectHasher, T> AsEnumerable() => new(_hasher, _bits.AsReadOnlySpan(), _popCount);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayEnumerator<TPerfectHasher, T> GetEnumerator() => new(_hasher, _bits.AsReadOnlySpan(), _popCount);
    [Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ReferenceTypeEnumerator(this);
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => new ReferenceTypeEnumerator(this);

    private sealed class ReferenceTypeEnumerator : IEnumerator<T>
    {
        private readonly TPerfectHasher _hasher;
        private BitArrayHelpers.SimpleBitEnumerator _bitEnumerator;

        public ReferenceTypeEnumerator(BitArraySet<TPerfectHasher, TBuffer, T> set)
        {
            _hasher = set._hasher;
            _bitEnumerator = new BitArrayHelpers.SimpleBitEnumerator(set._bits.AsReadOnlySpan().ToArray(), set._popCount);
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();
        public T Current => _hasher.UnHash(_bitEnumerator.Current);

        void IEnumerator.Reset() => throw new InvalidOperationException("Cannot reset");
        void IDisposable.Dispose() { }
        object IEnumerator.Current => Current;
    }

    private void GetBitsFromEnumerable(Span<uint> buffer, IEnumerable<T> other)
    {
        if (other is BitArraySet<TPerfectHasher, TBuffer, T> set)
        {
            set._bits.AsReadOnlySpan().CopyTo(buffer);
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
        BitArrayHelpers.UnionWith(_bits.AsSpan(), otherBitBuffer);
        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    public void UnionWith(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        BitArrayHelpers.UnionWith(_bits.AsSpan(), otherBits);
        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArrayHelpers.IntersectWith(_bits.AsSpan(), otherBitBuffer);
        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    public void IntersectWith(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        BitArrayHelpers.IntersectWith(_bits.AsSpan(), otherBits);
        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArrayHelpers.ExceptWith(_bits.AsSpan(), otherBitBuffer);
        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    public void ExceptWith(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        BitArrayHelpers.ExceptWith(_bits.AsSpan(), otherBits);
        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArrayHelpers.SymmetricExceptWith(_bits.AsSpan(), otherBitBuffer);
        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    public void SymmetricExceptWith(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        BitArrayHelpers.SymmetricExceptWith(_bits.AsSpan(), otherBits);
        _popCount = BitArrayHelpers.GetPopCount(_bits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.IsSubsetOf(_bits.AsReadOnlySpan(), otherBitBuffer);
    }

    [Pure]
    public bool IsSubsetOf(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        return BitArrayHelpers.IsSubsetOf(_bits.AsReadOnlySpan(), otherBits);
    }

    [Pure]
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.IsSubsetOf(otherBitBuffer, _bits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsSupersetOf(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        return BitArrayHelpers.IsSubsetOf(otherBits, _bits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.IsProperSubsetOf(_bits.AsReadOnlySpan(), otherBitBuffer);
    }

    [Pure]
    public bool IsProperSubsetOf(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        return BitArrayHelpers.IsProperSubsetOf(_bits.AsReadOnlySpan(), otherBits);
    }

    [Pure]
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.IsProperSubsetOf(otherBitBuffer, _bits.AsReadOnlySpan());
    }

    [Pure]
    public bool IsProperSupersetOf(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        return BitArrayHelpers.IsProperSubsetOf(otherBits, _bits.AsReadOnlySpan());
    }

    [Pure]
    public bool Overlaps(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.Overlaps(_bits.AsReadOnlySpan(), otherBitBuffer);
    }

    [Pure]
    public bool Overlaps(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        return BitArrayHelpers.Overlaps(_bits.AsReadOnlySpan(), otherBits);
    }

    [Pure]
    public bool SetEquals(IEnumerable<T> other)
    {
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        Span<uint> otherBitBuffer = bufferLength > MaxStackAllocSize
            ? new uint[bufferLength]
            : stackalloc uint[bufferLength];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArrayHelpers.SetEquals(_bits.AsReadOnlySpan(), otherBitBuffer);
    }

    [Pure]
    public bool SetEquals(BitArraySet<TPerfectHasher, TBuffer, T> other)
    {
        var otherBits = other._bits.AsReadOnlySpan();
        return BitArrayHelpers.SetEquals(_bits.AsReadOnlySpan(), otherBits);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;
}