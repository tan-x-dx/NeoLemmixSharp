using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class HashSetLookup<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly IEqualityComparer<TValue>? _valueComparer;
    private readonly Dictionary<TKey, HashSet<TValue>> _dictionary;

    public int KeyCount => _dictionary.Count;

    public int ValueCount
    {
        get
        {
            var count = 0;

            foreach (var (_, set) in _dictionary)
            {
                count += set.Count;
            }

            return count;
        }
    }

    public HashSetLookup(IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null)
    {
        _valueComparer = valueComparer;
        _dictionary = new Dictionary<TKey, HashSet<TValue>>(keyComparer);
    }

    public bool Add(TKey key, TValue value)
    {
        ref var setReference = ref CollectionsMarshal.GetValueRefOrAddDefault(_dictionary, key, out var exists);

        if (exists)
            return setReference!.Add(value);

        setReference = new HashSet<TValue>(_valueComparer) { value };
        return true;
    }

    public void Clear() => _dictionary.Clear();

    public bool RemoveAllByKey(TKey key) => _dictionary.Remove(key);
    public bool Remove(TKey key, TValue value)
    {
        if (!_dictionary.TryGetValue(key, out var collection))
            return false;

        var result = collection.Remove(value);

        if (collection.Count == 0)
        {
            _dictionary.Remove(key);
        }

        return result;
    }

    public Dictionary<TKey, HashSet<TValue>>.KeyCollection Keys => _dictionary.Keys;
    public bool TryGetValues(TKey key, [MaybeNullWhen(false)] out HashSet<TValue> values) => _dictionary.TryGetValue(key, out values);
    public bool Contains(TKey key) => _dictionary.ContainsKey(key);

    public HashSetLookupEnumerator GetEnumerator() => new(this);

    public ref struct HashSetLookupEnumerator
    {
        private Dictionary<TKey, HashSet<TValue>>.Enumerator _dictionaryEnumerator;

        public HashSetLookupEnumerator(HashSetLookup<TKey, TValue> lookup)
        {
            _dictionaryEnumerator = lookup._dictionary.GetEnumerator();
        }

        public bool MoveNext() => _dictionaryEnumerator.MoveNext();

        public Group Current
        {
            get
            {
                var (key, set) = _dictionaryEnumerator.Current;
                return new Group(key, set);
            }
        }
    }

    public readonly struct Group
    {
        public readonly TKey Key;
        private readonly HashSet<TValue> _items;

        public int Count => _items.Count;

        public Group(TKey key, HashSet<TValue> items)
        {
            Key = key;
            _items = items;
        }

        public HashSet<TValue>.Enumerator GetEnumerator() => _items.GetEnumerator();
    }
}