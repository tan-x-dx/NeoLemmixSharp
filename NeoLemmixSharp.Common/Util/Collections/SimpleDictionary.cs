using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using BitArray = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArray;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
{
    private readonly IPerfectHasher<TKey> _hasher;
    private readonly BitArray _bits;
    private readonly TValue[] _values;

    public int Count => _bits.PopCount;

    public SimpleDictionary(IPerfectHasher<TKey> hasher)
    {
        _hasher = hasher;
        _bits = new BitArray(hasher.NumberOfItems);
        _values = new TValue[hasher.NumberOfItems];
        Array.Clear(_values);
    }

    public void Add(TKey key, TValue value)
    {
        var index = _hasher.Hash(key);
        if (!_bits.SetBit(index))
            throw new ArgumentException("Key already added!", nameof(key));

        _values[index] = value;
    }

    public void Clear()
    {
        _bits.Clear();
        Array.Clear(_values);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var index in _bits)
        {
            var key = _hasher.UnHash(index);
            var value = _values[index];
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    public bool ContainsKey(TKey key)
    {
        var index = _hasher.Hash(key);
        return _bits.GetBit(index);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        var index = _hasher.Hash(key);
        value = _values[index];
        return _bits.GetBit(index);
    }

    public bool Remove(TKey key)
    {
        var index = _hasher.Hash(key);
        _values[index] = default!;
        return _bits.ClearBit(index);
    }

    public TValue this[TKey key]
    {
        get
        {
            var index = _hasher.Hash(key);
            if (!_bits.GetBit(index))
                throw new KeyNotFoundException();

            return _values[index];
        }
        set
        {
            var index = _hasher.Hash(key);
            _bits.SetBit(index);
            _values[index] = value;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);
    [Pure]
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => new Enumerator(this);
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly IPerfectHasher<TKey> _hasher;
        private readonly BitArray.ReferenceTypeBitEnumerator _enumerator;
        private readonly TValue[] _values;

        public Enumerator(SimpleDictionary<TKey, TValue> dictionary)
        {
            _hasher = dictionary._hasher;
            _enumerator = dictionary._bits.GetEnumerator();
            _values = dictionary._values;
        }

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();

        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                var index = _enumerator.Current;
                var key = _hasher.UnHash(index);
                var value = _values[index];
                return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        object IEnumerator.Current => Current;

        void IDisposable.Dispose()
        {
        }
    }

    [Pure]
    public TKey[] Keys
    {
        get
        {
            var result = new TKey[Count];
            var i = 0;

            foreach (var (key, _) in this)
            {
                result[i++] = key;
            }

            return result;
        }
    }

    [Pure]
    public TValue[] Values
    {
        get
        {
            var result = new TValue[Count];

            var i = 0;
            foreach (var (_, value) in this)
            {
                result[i++] = value;
            }

            return result;
        }
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
}