using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class BitArrayDictionary<TPerfectHasher, TBuffer, TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TPerfectHasher : IPerfectHasher<TKey>, IBitBufferCreator<TBuffer>
    where TBuffer : struct, IBitBuffer
    where TKey : notnull
{
#pragma warning disable IDE0044 // Add readonly modifier
    private TBuffer _bits;
#pragma warning restore IDE0044 // Add readonly modifier
    private int _popCount;
    private readonly TValue[] _values;
    private readonly TPerfectHasher _hasher;

    public int Count => _popCount;

    public BitArrayDictionary(TPerfectHasher hasher)
    {
        _hasher = hasher;
        _hasher.CreateBitBuffer(out _bits);
        var numberOfItems = hasher.NumberOfItems;

        BitArrayHelpers.ThrowIfInvalidCapacity(numberOfItems, _bits.Length);

        _bits.AsSpan().Clear();
        _popCount = 0;
        _values = new TValue[numberOfItems];
    }

    public void Add(TKey key, TValue value)
    {
        var index = _hasher.Hash(key);
        if (!BitArrayHelpers.SetBit(_bits.AsSpan(), index, ref _popCount))
            throw new ArgumentException("Key already added!", nameof(key));

        _values[index] = value;
    }

    public void Clear()
    {
        _bits.AsSpan().Clear();
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

    public void CopyKeysTo(Span<TKey> values)
    {
        var i = 0;
        var enumerator = new Enumerator(this);

        while (enumerator.MoveNext())
        {
            var key = enumerator.Current.Key;
            values[i++] = key;
        }
    }

    public void CopyValuesTo(Span<TValue> values)
    {
        var i = 0;
        var enumerator = new Enumerator(this);

        while (enumerator.MoveNext())
        {
            var value = enumerator.Current.Value;
            values[i++] = value;
        }
    }

    [Pure]
    public bool ContainsKey(TKey key)
    {
        var index = _hasher.Hash(key);
        return BitArrayHelpers.GetBit(_bits.AsReadOnlySpan(), index);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var index = _hasher.Hash(key);
        value = _values[index];
        return BitArrayHelpers.GetBit(_bits.AsReadOnlySpan(), index);
    }

    public bool Remove(TKey key)
    {
        var index = _hasher.Hash(key);
        _values[index] = default!;
        return BitArrayHelpers.ClearBit(_bits.AsSpan(), index, ref _popCount);
    }

    public TValue this[TKey key]
    {
        get
        {
            var index = _hasher.Hash(key);
            if (!BitArrayHelpers.GetBit(_bits.AsReadOnlySpan(), index))
                throw new KeyNotFoundException();

            return _values[index];
        }
        set
        {
            var index = _hasher.Hash(key);
            BitArrayHelpers.SetBit(_bits.AsSpan(), index, ref _popCount);
            _values[index] = value;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<TValue> _values;
        private BitArrayEnumerator _bitEnumerator;
        private readonly TPerfectHasher _hasher;

        public Enumerator(BitArrayDictionary<TPerfectHasher, TBuffer, TKey, TValue> dictionary)
        {
            _values = new ReadOnlySpan<TValue>(dictionary._values);
            _bitEnumerator = new BitArrayEnumerator(dictionary._bits.AsReadOnlySpan(), dictionary._popCount);
            _hasher = dictionary._hasher;
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

    private sealed class ReferenceTypeEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly TPerfectHasher _hasher;
        private readonly TValue[] _values;
        private BitArrayHelpers.SimpleBitEnumerator _enumerator;

        public ReferenceTypeEnumerator(BitArrayDictionary<TPerfectHasher, TBuffer, TKey, TValue> dictionary)
        {
            _hasher = dictionary._hasher;
            _enumerator = new BitArrayHelpers.SimpleBitEnumerator(dictionary._bits.AsReadOnlySpan().ToArray(), dictionary._popCount);
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
            CopyKeysTo(result);
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
            CopyValuesTo(result);
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