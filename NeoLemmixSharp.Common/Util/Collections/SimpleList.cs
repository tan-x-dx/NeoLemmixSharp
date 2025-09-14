namespace NeoLemmixSharp.Common.Util.Collections;

/// <summary>
/// A simple list implementation that can grow as necessary.
/// </summary>
public sealed class SimpleList<T>
    where T : notnull
{
    private T[] _items;
    private int _size;

    public int Count => _size;
    public int Capacity => _items.Length;

    public SimpleList(int capacity)
    {
        _items = Helpers.GetArrayForSize<T>(capacity);
    }

    public void EnsureCapacity(int requiredCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(requiredCapacity);
        ArgumentOutOfRangeException.ThrowIfLessThan(requiredCapacity, Capacity);

        ResizeBackingArray(requiredCapacity);
    }

    public Span<T> AsSpan() => new(_items, 0, _size);
    public ReadOnlySpan<T> AsReadOnlySpan() => new(_items, 0, _size);

    public ref T this[int index]
    {
        get
        {
            if ((uint)index > (uint)_size)
                throw new IndexOutOfRangeException();

            return ref _items[index];
        }
    }

    public void Add(T item)
    {
        if (_size == _items.Length)
        {
            var newSize = _items.Length == 0
                ? 8
                : _items.Length << 1;
            ResizeBackingArray(newSize);
        }

        _items[_size++] = item;
    }

    public void AddRange(ICollection<T> collection)
    {
        var newSize = _size + collection.Count;
        if (newSize > _items.Length)
        {
            ResizeBackingArray(newSize);
        }

        collection.CopyTo(_items, _size);
        _size = newSize;
    }

    private void ResizeBackingArray(int newSize)
    {
        var newArray = new T[newSize];
        new ReadOnlySpan<T>(_items).CopyTo(newArray);
        _items = newArray;
    }

    public void Clear()
    {
        new Span<T>(_items).Clear();
        _size = 0;
    }
}
