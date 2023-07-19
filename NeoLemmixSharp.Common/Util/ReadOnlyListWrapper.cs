using System.Collections;
using System.Diagnostics;

namespace NeoLemmixSharp.Common.Util;

public sealed class ReadOnlyListWrapper<T> : IList<T>, IReadOnlyList<T>
{
    private readonly List<T> _list;

    public int Count => _list.Count;

    private static NotSupportedException CannotMutateException => new("Cannot modify the contents of a ReadOnlyListWrapper");

    public ReadOnlyListWrapper(List<T> list)
    {
        _list = list;
    }

    public T this[int index] => _list[index];

    public bool Contains(T item) => _list.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public int IndexOf(T item) => _list.IndexOf(item);

    public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();

    T IList<T>.this[int index]
    {
        get => _list[index];
        set => throw CannotMutateException;
    }

    void ICollection<T>.Add(T item) => throw CannotMutateException;
    void ICollection<T>.Clear() => throw CannotMutateException;
    bool ICollection<T>.Remove(T item) => throw CannotMutateException;

    void IList<T>.Insert(int index, T item) => throw CannotMutateException;
    void IList<T>.RemoveAt(int index) => throw CannotMutateException;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => true;
}