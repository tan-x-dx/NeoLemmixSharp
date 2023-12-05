using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;

namespace NeoLemmixSharp.Common.Util.Identity;

public sealed class ExtendedEnumTypeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : class, IExtendedEnumType<TKey>
{
    private readonly SimpleSet<TKey> _keys = ExtendedEnumTypeComparer<TKey>.CreateSimpleSet();
    private readonly TValue[] _values = new TValue[TKey.NumberOfItems];

    public int Count => _keys.Count;

    public void Add(TKey key, TValue value)
    {
        if (!_keys.Add(key))
            throw new ArgumentException("Key already added!", nameof(key));

        _values[key.Id] = value;
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
            var value = _values[key.Id];
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    public bool ContainsKey(TKey key)
    {
        return _keys.Contains(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (_keys.Contains(key))
        {
            value = _values[key.Id];
            return true;
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        if (!_keys.Remove(key))
            return false;

        _values[key.Id] = default;
        return true;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (!_keys.Contains(key))
                throw new KeyNotFoundException();

            return _values[key.Id];
        }
        set
        {
            _keys.Add(key);
            _values[key.Id] = value;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReferenceTypeEnumerator GetReferenceTypeEnumerator() => new(this);

    [Pure]
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => new ReferenceTypeEnumerator(this);

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => new ReferenceTypeEnumerator(this);

    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<TValue> _values;
        private BitBasedEnumerator<TKey> _bitEnumerator;

        public Enumerator(ExtendedEnumTypeDictionary<TKey, TValue> dictionary)
        {
            _values = dictionary.Values;
            _bitEnumerator = dictionary._keys.GetEnumerator();
        }

        public bool MoveNext() => _bitEnumerator.MoveNext();

        public readonly KeyValuePair<TKey, TValue> Current
        {
            get
            {
                var key = _bitEnumerator.Current;
                return new KeyValuePair<TKey, TValue>(key, _values[key.Id]);
            }
        }
    }

    public sealed class ReferenceTypeEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly SimpleSet<TKey>.ReferenceTypeEnumerator _enumerator;
        private readonly TValue[] _values;

        public ReferenceTypeEnumerator(ExtendedEnumTypeDictionary<TKey, TValue> dictionary)
        {
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

                return new KeyValuePair<TKey, TValue>(key, _values[key.Id]);
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
    public ReadOnlySpan<TValue> Values => new(_values);

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _keys;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _values;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TValue> IDictionary<TKey, TValue>.Values => _values;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
}