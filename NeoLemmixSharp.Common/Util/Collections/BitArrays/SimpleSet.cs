using System.Collections;
using System.Diagnostics;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class SimpleSet<T> : ICollection<T>, IReadOnlyCollection<T>
{
    private readonly ISimpleHasher<T> _hasher;
    private readonly LargeBitArray _bits;

    public SimpleSet(ISimpleHasher<T> hasher)
    {
        _hasher = hasher;
        _bits = new LargeBitArray(_hasher.NumberOfItems);
    }

    public int Count => _bits.Count;

    public bool Add(T item)
    {
        var hash = _hasher.Hash(item);
        return _bits.SetBit(hash);
    }

    void ICollection<T>.Add(T item) => Add(item);

    public void Clear() => _bits.Clear();

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

    public SimpleSetEnumerator GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new SimpleSetEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new SimpleSetEnumerator(this);

    public struct SimpleSetEnumerator : IEnumerator<T>
    {
        private readonly ISimpleHasher<T> _hasher;
        private LargeBitArray.Enumerator _bitEnumerator;

        public SimpleSetEnumerator(SimpleSet<T> set)
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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;
}

public interface ISimpleHasher<T>
{
    int NumberOfItems { get; }

    int Hash(T item);
    T Unhash(int hash);
}