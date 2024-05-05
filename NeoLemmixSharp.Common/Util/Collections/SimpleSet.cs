using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using BitArray = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArray;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class SimpleSet<T> : ISet<T>, IReadOnlySet<T>, IItemCountListener
{
    private const int MaxStackAllocSize = 64;

    private readonly IPerfectHasher<T> _hasher;
    private uint[] _bits;
    private int _popCount;

    public SimpleSet(IPerfectHasher<T> hasher, bool fullSet = false)
    {
        _hasher = hasher;
        var numberOfItems = hasher.NumberOfItems;
        _bits = BitArray.CreateBitArray(numberOfItems, fullSet);
        _popCount = fullSet
            ? numberOfItems
            : 0;
    }

    /// <summary>
    /// The footprint of the underlying BitArray - how many uints it logically represents.
    /// </summary>
    public int Size => _bits.Length;
    public int Count => _popCount;

    public bool Add(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArray.SetBit(new Span<uint>(_bits), hash, ref _popCount);
    }

    void ICollection<T>.Add(T item) => Add(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        new Span<uint>(_bits).Clear();
        _popCount = 0;
    }

    [Pure]
    public bool Contains(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArray.GetBit(new ReadOnlySpan<uint>(_bits), hash);
    }

    public bool Remove(T item)
    {
        var hash = _hasher.Hash(item);
        return BitArray.ClearBit(new Span<uint>(_bits), hash, ref _popCount);
    }

    public void OnNumberOfItemsChanged(int numberOfItems)
    {
        BitArray.SetLength(ref _bits, numberOfItems);
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
        var iterator = new BitBasedEnumerator<T>(_hasher, new ReadOnlySpan<uint>(_bits), _popCount);
        while (iterator.MoveNext())
        {
            array[arrayIndex++] = iterator.Current;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleSetEnumerable<T> ToSimpleEnumerable() => new(_hasher, new ReadOnlySpan<uint>(_bits), _popCount);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBasedEnumerator<T> GetEnumerator() => new(_hasher, new ReadOnlySpan<uint>(_bits), _popCount);
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
            _bitEnumerator = new BitArray.ReferenceTypeBitEnumerator(set._bits, set._popCount);
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();
        public T Current => _hasher.UnHash(_bitEnumerator.Current);

        void IEnumerator.Reset() => throw new InvalidOperationException("Cannot reset");
        void IDisposable.Dispose() { }
        object IEnumerator.Current => Current!;
    }

    private void GetBitsFromEnumerable(Span<uint> buffer, IEnumerable<T> other)
    {
        if (other is SimpleSet<T> set)
        {
            new ReadOnlySpan<uint>(set._bits).CopyTo(buffer);
            return;
        }

        foreach (var item in other)
        {
            var index = _hasher.Hash(item);
            BitArray.SetBit(buffer, index);
        }
    }

    public void UnionWith(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArray.UnionWith(new Span<uint>(_bits), otherBitBuffer, ref _popCount);
    }

    public void UnionWith(SimpleSet<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArray.UnionWith(new Span<uint>(_bits), otherBitBuffer, ref _popCount);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArray.IntersectWith(new Span<uint>(_bits), otherBitBuffer, ref _popCount);
    }

    public void IntersectWith(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        BitArray.IntersectWith(new Span<uint>(_bits), new ReadOnlySpan<uint>(otherBits), ref _popCount);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArray.ExceptWith(new Span<uint>(_bits), otherBitBuffer, ref _popCount);
    }

    public void ExceptWith(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        BitArray.ExceptWith(new Span<uint>(_bits), new ReadOnlySpan<uint>(otherBits), ref _popCount);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        BitArray.SymmetricExceptWith(new Span<uint>(_bits), otherBitBuffer, ref _popCount);
    }

    public void SymmetricExceptWith(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        BitArray.SymmetricExceptWith(new Span<uint>(_bits), new ReadOnlySpan<uint>(otherBits), ref _popCount);
    }

    [Pure]
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArray.IsSubsetOf(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool IsSubsetOf(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return BitArray.IsSubsetOf(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArray.IsSupersetOf(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool IsSupersetOf(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return BitArray.IsSupersetOf(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArray.IsProperSubsetOf(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool IsProperSubsetOf(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return BitArray.IsProperSubsetOf(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArray.IsProperSupersetOf(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool IsProperSupersetOf(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return BitArray.IsProperSupersetOf(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool Overlaps(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArray.Overlaps(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool Overlaps(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return BitArray.Overlaps(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [Pure]
    public bool SetEquals(IEnumerable<T> other)
    {
        var bufferSize = _hasher.NumberOfItems;
        Span<uint> otherBitBuffer = bufferSize > MaxStackAllocSize
            ? new uint[bufferSize]
            : stackalloc uint[bufferSize];

        GetBitsFromEnumerable(otherBitBuffer, other);
        return BitArray.SetEquals(new ReadOnlySpan<uint>(_bits), otherBitBuffer);
    }

    [Pure]
    public bool SetEquals(SimpleSet<T> other)
    {
        var otherBits = other._bits;
        return BitArray.SetEquals(new ReadOnlySpan<uint>(_bits), new ReadOnlySpan<uint>(otherBits));
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;
}