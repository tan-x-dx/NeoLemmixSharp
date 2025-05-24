using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class ListLookup<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull, IEquatable<TKey>
{
    private readonly KeyValuePair<TKey, TValue>[] _data;
    private int _size;

    public int Count => _size;
    public int Capacity => _data.Length;

    public ListLookup(int capacity)
    {
        _data = new KeyValuePair<TKey, TValue>[capacity];
    }

    public void Add(TKey key, TValue value)
    {
        var index = FindIndex(key);
        if (index >= 0)
            throw new ArgumentException("Key already added!", nameof(key));

        _data[_size++] = new KeyValuePair<TKey, TValue>(key, value);
    }

    public void Clear()
    {
        new Span<KeyValuePair<TKey, TValue>>(_data).Clear();
        _size = 0;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        for (var i = 0; i < _size; i++)
        {
            array[arrayIndex + i] = _data[i];
        }
    }

    public bool ContainsKey(TKey key)
    {
        return FindIndex(key) >= 0;
    }

    public bool Remove(TKey key)
    {
        var index = FindIndex(key);
        if (index < 0)
            return false;

        _size--;
        _data[index] = _data[_size];
        _data[_size] = default;

        if (_size == 0)
            Clear();

        return true;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var index = FindIndex(key);
        if (index < 0)
        {
            value = default;
            return false;
        }

        value = _data[index].Value;
        return true;
    }

    public TValue this[TKey key]
    {
        get
        {
            var index = FindIndex(key);
            if (index < 0)
                throw new KeyNotFoundException();
            return _data[index].Value;
        }
        set
        {
            var index = FindIndex(key);
            if (index < 0)
            {
                _data[_size++] = new KeyValuePair<TKey, TValue>(key, value);
                return;
            }

            _data[index] = new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    private int FindIndex(TKey key)
    {
        for (var i = 0; i < _size; i++)
        {
            if (key.Equals(_data[i].Key))
                return i;
        }

        return -1;
    }

    public TKey[] Keys
    {
        get
        {
            var size = _size;
            if (size == 0)
                return [];
            var result = new TKey[size];
            for (var i = 0; i < size; i++)
                result[i] = _data[i].Key;
            return result;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var size = _size;
            if (size == 0)
                return [];
            var result = new TValue[size];
            for (var i = 0; i < size; i++)
                result[i] = _data[i].Value;
            return result;
        }
    }

    public Enumerator GetEnumerator() => new(this);

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly ListLookup<TKey, TValue> _lookup;
        private int _index;
        private KeyValuePair<TKey, TValue> _current;

        internal Enumerator(ListLookup<TKey, TValue> lookup)
        {
            _lookup = lookup;
            _index = 0;
            _current = default;
        }

        public bool MoveNext()
        {
            var localLookup = _lookup;

            if ((uint)_index < (uint)localLookup._size)
            {
                _current = localLookup._data[_index];
                _index++;
                return true;
            }
            return MoveNextRare();
        }

        private bool MoveNextRare()
        {
            _index = _lookup._size + 1;
            _current = default;
            return false;
        }

        public readonly KeyValuePair<TKey, TValue> Current => _current;

        readonly object? IEnumerator.Current => Current;
        readonly void IEnumerator.Reset() { }
        readonly void IDisposable.Dispose() { }
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
