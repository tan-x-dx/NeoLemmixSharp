using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class FrameOrderedList<TFrameOrderedData>
    where TFrameOrderedData : struct, IFrameOrderedData
{
    private TFrameOrderedData[] _items;
    private int _count;

    public FrameOrderedList(int initialCapacity)
    {
        _items = new TFrameOrderedData[initialCapacity];
    }

    public int Count => _count;

    [Pure]
    public ref readonly TFrameOrderedData this[int index] => ref _items[index];

    [Pure]
    public ReadOnlySpan<TFrameOrderedData> Slice(int start, int length)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start), "Negative start index");
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(start), "Negative length");
        if (_count - start < length)
            throw new ArgumentOutOfRangeException(nameof(start), "Start index with length is out of bounds");

        return new ReadOnlySpan<TFrameOrderedData>(_items, start, length);
    }

    [Pure]
    public ReadOnlySpan<TFrameOrderedData> SliceToEnd(int start)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start), "Negative start index");

        return new ReadOnlySpan<TFrameOrderedData>(_items, start, Math.Max(0, _count - start));
    }

    [Pure]
    public ReadOnlySpan<TFrameOrderedData> GetSliceBackTo(int frame)
    {
        var index = GetSmallestIndexOfFrame(frame);

        return SliceToEnd(index);
    }

    /// <summary>
    /// Returns the smallest index such that the data at that index has a frame equal to or exceeding the input parameter
    /// <para>
    /// Binary search algorithm - O(log n)
    /// </para>
    /// </summary>
    [Pure]
    private int GetSmallestIndexOfFrame(int frame)
    {
        if (_count == 0)
            return 0;

        var upperTestIndex = _count;
        var lowerTestIndex = 0;

        while (upperTestIndex - lowerTestIndex > 1)
        {
            var bestGuess = (lowerTestIndex + upperTestIndex) >> 1;
            ref readonly var test = ref _items[bestGuess];

            if (test.Frame >= frame)
            {
                upperTestIndex = bestGuess;
            }
            else
            {
                lowerTestIndex = bestGuess;
            }
        }

        ref readonly var test1 = ref _items[lowerTestIndex];
        return test1.Frame >= frame
            ? lowerTestIndex
            : upperTestIndex;
    }

    public ref TFrameOrderedData GetNewDataRef()
    {
        var arraySize = _items.Length;
        if (_count == arraySize)
        {
            var newArray = new TFrameOrderedData[arraySize * 2];
            new ReadOnlySpan<TFrameOrderedData>(_items).CopyTo(newArray);

            _items = newArray;
        }

        return ref _items[_count++];
    }

    [Pure]
    public int LatestFrameWithData()
    {
        if (_count == 0)
            return -1;

        ref readonly var data = ref _items[_count - 1];

        return data.Frame;
    }
}