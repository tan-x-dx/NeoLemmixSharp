using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Common.Util;

public sealed class ReadOnlyDictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary;

    public int Count => _dictionary.Count;

    private static NotSupportedException CannotMutateException => new("Cannot modify the contents of a ReadOnlyDictionaryWrapper");

    public ReadOnlyDictionaryWrapper(Dictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

    public TValue this[TKey key] => _dictionary[key];

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dictionary.TryGetValue(key, out value);

    public Dictionary<TKey, TValue>.KeyCollection Keys => _dictionary.Keys;
    public Dictionary<TKey, TValue>.ValueCollection Values => _dictionary.Values;
    public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => _dictionary.GetEnumerator();

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => _dictionary[key];
        set => throw CannotMutateException;
    }

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => throw CannotMutateException;
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => throw CannotMutateException;
    void ICollection<KeyValuePair<TKey, TValue>>.Clear() => throw CannotMutateException;
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    bool IDictionary<TKey, TValue>.Remove(TKey key) => throw CannotMutateException;
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => throw CannotMutateException;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => _dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;
}