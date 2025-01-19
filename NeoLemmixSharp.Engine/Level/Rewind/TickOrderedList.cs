using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class TickOrderedList<TTickOrderedData>
    where TTickOrderedData : unmanaged, ITickOrderedData
{
    private TTickOrderedData[] _items;
    private int _count;

    public TickOrderedList(int initialCapacity)
    {
        _items = new TTickOrderedData[initialCapacity];
    }

    public int Count => _count;

    [Pure]
    public ref readonly TTickOrderedData this[int index] => ref _items[index];

    [Pure]
    public ReadOnlySpan<TTickOrderedData> Slice(int start, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        if (_count - start < length)
            throw new ArgumentOutOfRangeException(nameof(start), "Start index with length is out of bounds");

        return new ReadOnlySpan<TTickOrderedData>(_items, start, length);
    }

    [Pure]
    public ReadOnlySpan<TTickOrderedData> GetSliceToEnd(int start)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);

        return new ReadOnlySpan<TTickOrderedData>(_items, start, Math.Max(0, _count - start));
    }

    public ReadOnlySpan<TTickOrderedData> RewindBackTo(int tick)
    {
        var index = _count == 0
            ? 0
            : GetSmallestIndexOfTick(tick);

        var result = GetSliceToEnd(index);

        _count = index;

        return result;
    }

    [Pure]
    public bool HasDataForTick(int tick)
    {
        if (_count == 0)
            return false;

        var index = GetSmallestIndexOfTick(tick);

        ref readonly var data = ref _items[index];

        return data.TickNumber == tick;
    }

    [Pure]
    public ref readonly TTickOrderedData TryGetDataForTick(int tick)
    {
        if (_count == 0)
            return ref Unsafe.NullRef<TTickOrderedData>();

        var index = GetSmallestIndexOfTick(tick);

        ref readonly var data = ref _items[index];

        if (data.TickNumber == tick)
            return ref data;

        return ref Unsafe.NullRef<TTickOrderedData>();
    }

    /// <summary>
    /// Returns the smallest index such that the data at that index has a tick equal to or exceeding the input parameter
    /// <para>
    /// Binary search algorithm - O(log n)
    /// </para>
    /// </summary>
    [Pure]
    private int GetSmallestIndexOfTick(int tick)
    {
        var upperTestIndex = _count;
        var lowerTestIndex = 0;

        while (upperTestIndex - lowerTestIndex > 1)
        {
            var bestGuess = (lowerTestIndex + upperTestIndex) >> 1;
            ref readonly var test = ref _items[bestGuess];

            if (test.TickNumber >= tick)
            {
                upperTestIndex = bestGuess;
            }
            else
            {
                lowerTestIndex = bestGuess;
            }
        }

        ref readonly var test1 = ref _items[lowerTestIndex];
        return test1.TickNumber >= tick
            ? lowerTestIndex
            : upperTestIndex;
    }

    public ref TTickOrderedData GetNewDataRef()
    {
        var arraySize = _items.Length;
        if (_count == arraySize)
        {
            var newArray = new TTickOrderedData[arraySize * 2];
            new ReadOnlySpan<TTickOrderedData>(_items).CopyTo(newArray);

            _items = newArray;
        }

        return ref _items[_count++];
    }

    [Pure]
    public int LatestTickWithData()
    {
        if (_count == 0)
            return -1;

        ref readonly var data = ref _items[_count - 1];

        return data.TickNumber;
    }
}