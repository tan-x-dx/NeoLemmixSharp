using NeoLemmixSharp.Common.Util.Collections;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Common.Util;

public ref struct ArrayListWrapper<T>
{
    private readonly T[] _array;

    private int _size;

    public ArrayListWrapper(int capacity)
    {
        _array = CollectionsHelper.GetArrayForSize<T>(capacity);
        _size = 0;
    }

    public readonly int Length => _array.Length;
    public readonly int Count => _size;

    public void Add(T item)
    {
        var size = _size;
        if ((uint)size < (uint)_array.Length)
        {
            _size = size + 1;
            _array[size] = item;

            return;
        }

        ThrowReachedCapacityException();
    }

    [DoesNotReturn]
    private static void ThrowReachedCapacityException() => throw new InvalidOperationException("This array is full!");

    public readonly Span<T> AsSpan() => new(_array, 0, _size);

    public readonly T[] GetArray() => _array;
}
