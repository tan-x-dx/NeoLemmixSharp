using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
{
    private readonly IPerfectHasher<TKey> _hasher;
    private readonly SimpleSet<TKey> _keys;
    private readonly TValue[] _values;

    public int Count => _keys.Count;

    public SimpleDictionary(IPerfectHasher<TKey> hasher)
    {
        _hasher = hasher;
        _keys = new SimpleSet<TKey>(hasher);
        _values = new TValue[hasher.NumberOfItems];
    }

    public void Add(TKey key, TValue value)
    {
        if (!_keys.Add(key))
            throw new ArgumentException("Key already added!", nameof(key));

        var index = _hasher.Hash(key);
        _values[index] = value;
    }

    public void Clear()
    {
        _keys.Clear();
        Array.Clear(_values);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var key in _keys)
        {
            var index = _hasher.Hash(key);
            var value = _values[index];
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    public bool ContainsKey(TKey key)
    {
        return _keys.Contains(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        var index = _hasher.Hash(key);
        value = _values[index];
        return _keys.Contains(key);
    }

    public bool Remove(TKey key)
    {
        var index = _hasher.Hash(key);
        _values[index] = default!;
        return _keys.Remove(key);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (!_keys.Contains(key))
                throw new KeyNotFoundException();

            var index = _hasher.Hash(key);
            return _values[index];
        }
        set
        {
            _keys.Add(key);
            var index = _hasher.Hash(key);
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
        private readonly SimpleSet<TKey>.ReferenceTypeEnumerator _enumerator;
        private readonly TValue[] _values;

        public Enumerator(SimpleDictionary<TKey, TValue> dictionary)
        {
            _hasher = dictionary._hasher;
            _enumerator = dictionary._keys.GetReferenceTypeEnumerator();
            _values = dictionary._values;
        }

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();

        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                var key = _enumerator.Current;
                var index = _hasher.Hash(key);
                return new KeyValuePair<TKey, TValue>(key, _values[index]);
            }
        }

        object IEnumerator.Current => Current;

        void IDisposable.Dispose()
        {
        }
    }

    [Pure]
    public SimpleSetEnumerable<TKey> Keys => _keys.ToSimpleEnumerable();
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
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _keys;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
}