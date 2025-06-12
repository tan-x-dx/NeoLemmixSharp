namespace NeoLemmixSharp.Common.Util.Collections;

/// <summary>
/// A simple list implementation that can grow as necessary.
/// It does not guarantee ordering of items.
/// </summary>
public sealed class SimpleList<T>
{
    private T[] _items;

    public int Count { get; private set; }

    public SimpleList(int capacity)
    {
        _items = Helpers.GetArrayForSize<T>(capacity);
    }

    public Span<T> AsSpan() => new(_items, 0, Count);
    public ReadOnlySpan<T> AsReadOnlySpan() => new(_items, 0, Count);

    public void Add(T item)
    {
        if (Count == _items.Length)
        {
            var newSize = _items.Length == 0
                ? 8
                : _items.Length << 1;
            ResizeBackingArray(newSize);
        }

        _items[Count++] = item;
    }

    public void AddRange(ICollection<T> collection)
    {
        var newSize = Count + collection.Count;
        if (newSize > _items.Length)
        {
            ResizeBackingArray(newSize);
        }

        collection.CopyTo(_items, Count);
        Count = newSize;
    }

    private void ResizeBackingArray(int newSize)
    {
        var newArray = new T[newSize];
        new ReadOnlySpan<T>(_items).CopyTo(newArray);
        _items = newArray;
    }

    private int IndexOf(T item)
    {
        var index = 0;

        while (index < Count)
        {
            var testItem = _items[index];
            if (testItem.Equals(item))
                return index;
            index++;
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
        _items[Count] = default!;

        if (Count == 0)
            Clear();

        return true;
    }
}
