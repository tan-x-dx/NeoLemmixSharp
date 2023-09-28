using NeoLemmixSharp.Common.Util.Identity;

namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class SimpleList<T>
    where T : class, IIdEquatable<T>
{
    private readonly T[] _items;

    public int Count { get; private set; }

    public SimpleList(int capacity)
    {
        _items = new T[capacity];
    }

    public ReadOnlySpan<T> AsSpan() => new(_items, 0, Count);

    public void Add(T item)
    {
        if (Count == _items.Length)
            throw new InvalidOperationException("Already reached max capacity!");

        if (Contains(item))
            throw new InvalidOperationException("Already contains item!");

        _items[Count++] = item;
    }

    private int IndexOf(T item)
    {
        var index = Count - 1;

        while (index >= 0)
        {
            if (item == _items[index])
                return index;
            index--;
        }

        return -1;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public void Clear()
    {
        Array.Clear(_items, 0, _items.Length);
        Count = 0;
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index == -1)
            return false;

        Count--;
        _items[index] = _items[Count];

        if (Count > 0)
            return true;

        Clear();

        return true;
    }
}