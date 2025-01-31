namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class SimpleList<T>
    where T : class, IEquatable<T>
{
    private readonly T[] _items;

    public int Count { get; private set; }

    public SimpleList(int capacity)
    {
        _items = CollectionsHelper.GetArrayForSize<T>(capacity);
    }

    public ReadOnlySpan<T> AsReadOnlySpan() => new(_items, 0, Count);

    public void Add(T item)
    {
        if (Count == _items.Length)
            throw new InvalidOperationException("Already reached max capacity!");

        if (IndexOf(item) >= 0)
            throw new InvalidOperationException("Already contains item!");

        _items[Count++] = item;
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
        _items[Count] = null!;

        if (Count > 0)
            return true;

        Clear();

        return true;
    }
}