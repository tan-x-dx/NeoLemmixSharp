using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class SimpleDictionary<TPerfectHasher, TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TPerfectHasher : IPerfectHasher<TKey>
    where TKey : notnull
{
    private readonly TPerfectHasher _hasher;
    private readonly uint[] _bits;
    private readonly TValue[] _values;
    private int _popCount;

    public int Count => _popCount;

    public SimpleDictionary(TPerfectHasher hasher)
    {
        _hasher = hasher;
        var numberOfItems = hasher.NumberOfItems;
        _bits = BitArrayHelpers.CreateBitArray(numberOfItems, false);
        _popCount = 0;
        _values = new TValue[numberOfItems];
    }

    public void Add(TKey key, TValue value)
    {
        var index = _hasher.Hash(key);
        if (!BitArrayHelpers.SetBit(new Span<uint>(_bits), index, ref _popCount))
            throw new ArgumentException("Key already added!", nameof(key));

        _values[index] = value;
    }

    public void Clear()
    {
        new Span<uint>(_bits).Clear();
        new Span<TValue>(_values).Clear();
        _popCount = 0;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        var enumerator = new Enumerator(this);
        while (enumerator.MoveNext())
        {
            array[arrayIndex++] = enumerator.Current;
        }
    }

    [Pure]
    public bool ContainsKey(TKey key)
    {
        var index = _hasher.Hash(key);
        return BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_bits), index);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var index = _hasher.Hash(key);
        value = _values[index];
        return BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_bits), index);
    }

    public bool Remove(TKey key)
    {
        var index = _hasher.Hash(key);
        _values[index] = default!;
        return BitArrayHelpers.ClearBit(new Span<uint>(_bits), index, ref _popCount);
    }

    public TValue this[TKey key]
    {
        [Pure]
        get
        {
            var index = _hasher.Hash(key);
            if (!BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(_bits), index))
                throw new KeyNotFoundException();

            return _values[index];
        }
        set
        {
            var index = _hasher.Hash(key);
            BitArrayHelpers.SetBit(new Span<uint>(_bits), index, ref _popCount);
            _values[index] = value;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    public ref struct Enumerator
    {
        private readonly TPerfectHasher _hasher;
        private readonly ReadOnlySpan<TValue> _values;
        private BitBasedEnumerator _bitEnumerator;

        public Enumerator(SimpleDictionary<TPerfectHasher, TKey, TValue> dictionary)
        {
            _hasher = dictionary._hasher;
            _values = new ReadOnlySpan<TValue>(dictionary._values);
            _bitEnumerator = new BitBasedEnumerator(dictionary._bits, dictionary._popCount);
        }

        [DebuggerStepThrough]
        public bool MoveNext() => _bitEnumerator.MoveNext();

        public readonly KeyValuePair<TKey, TValue> Current
        {
            get
            {
                var index = _bitEnumerator.Current;
                var key = _hasher.UnHash(index);
                var value = _values[index];
                return new KeyValuePair<TKey, TValue>(key, value);
            }
        }
    }

    [Pure]
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => new ReferenceTypeEnumerator(this);
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => new ReferenceTypeEnumerator(this);

    public sealed class ReferenceTypeEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly TPerfectHasher _hasher;
        private readonly BitArrayHelpers.ReferenceTypeBitEnumerator _enumerator;
        private readonly TValue[] _values;

        public ReferenceTypeEnumerator(SimpleDictionary<TPerfectHasher, TKey, TValue> dictionary)
        {
            _hasher = dictionary._hasher;
            _enumerator = new BitArrayHelpers.ReferenceTypeBitEnumerator(dictionary._bits, dictionary._popCount);
            _values = dictionary._values;
        }

        [DebuggerStepThrough]
        public bool MoveNext() => _enumerator.MoveNext();

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

        void IEnumerator.Reset() => throw new InvalidOperationException("Cannot reset");
        object IEnumerator.Current => Current;
        void IDisposable.Dispose() { }
    }

    [Pure]
    public TKey[] Keys
    {
        get
        {
            if (_popCount == 0)
                return [];

            var result = new TKey[_popCount];
            var i = 0;
            var enumerator = new Enumerator(this);

            while (enumerator.MoveNext())
            {
                var (key, _) = enumerator.Current;
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
            if (_popCount == 0)
                return [];

            var result = new TValue[_popCount];
            var i = 0;
            var enumerator = new Enumerator(this);

            while (enumerator.MoveNext())
            {
                var (_, value) = enumerator.Current;
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