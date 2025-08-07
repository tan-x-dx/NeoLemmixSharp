using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class TickOrderedList<TTickOrderedData> : IDisposable
    where TTickOrderedData : unmanaged, ITickOrderedData
{
    private RawArray _buffer;
    private int _bufferLength;
    private int _count;

    public TickOrderedList(int initialCapacity)
    {
        _bufferLength = initialCapacity;
        _buffer = CreateBuffer(_bufferLength);
    }

    private static unsafe RawArray CreateBuffer(int initialCapacity)
    {
        var bufferLengthInBytes = initialCapacity * sizeof(TTickOrderedData);
        return new RawArray(bufferLengthInBytes);
    }

    public int Count => _count;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe ReadOnlySpan<TTickOrderedData> GetReadOnlySpan(int start, int length)
    {
        TTickOrderedData* pointer = (TTickOrderedData*)_buffer.Handle + start;

        return new ReadOnlySpan<TTickOrderedData>(pointer, length);
    }

    [Pure]
    public ReadOnlySpan<TTickOrderedData> Slice(int start, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        if (_count - start < length)
            throw new ArgumentOutOfRangeException(nameof(start), "Start index with length is out of bounds");

        return GetReadOnlySpan(start, length);
    }

    [Pure]
    public ReadOnlySpan<TTickOrderedData> GetSliceToEnd(int start)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);

        return GetReadOnlySpan(start, Math.Max(0, _count - start));
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
    public unsafe bool HasDataForTick(int tick)
    {
        if (_count == 0)
            return false;

        var index = GetSmallestIndexOfTick(tick);

        TTickOrderedData* pointer = (TTickOrderedData*)_buffer.Handle + index;

        return pointer->TickNumber == tick;
    }

    [Pure]
    public unsafe ref readonly TTickOrderedData TryGetDataForTick(int tick)
    {
        if (_count == 0)
            return ref Unsafe.NullRef<TTickOrderedData>();

        var index = GetSmallestIndexOfTick(tick);

        TTickOrderedData* pointer = (TTickOrderedData*)_buffer.Handle + index;

        if (pointer->TickNumber == tick)
            return ref *pointer;

        return ref Unsafe.NullRef<TTickOrderedData>();
    }

    /// <summary>
    /// Returns the smallest index such that the data at that index has a tick equal to or exceeding the input parameter
    /// <para>
    /// Binary search algorithm - O(log n)
    /// </para>
    /// </summary>
    [Pure]
    private unsafe int GetSmallestIndexOfTick(int tick)
    {
        var upperTestIndex = _count - 1;
        var lowerTestIndex = 0;

        TTickOrderedData* basePointer = (TTickOrderedData*)_buffer.Handle;

        while (upperTestIndex - lowerTestIndex > 1)
        {
            var bestGuessIndex = (lowerTestIndex + upperTestIndex) >>> 1;
            TTickOrderedData* pointer = basePointer + bestGuessIndex;

            if (pointer->TickNumber >= tick)
            {
                upperTestIndex = bestGuessIndex;
            }
            else
            {
                lowerTestIndex = bestGuessIndex;
            }
        }

        basePointer += lowerTestIndex;
        return basePointer->TickNumber >= tick
            ? lowerTestIndex
            : upperTestIndex;
    }

    public unsafe ref TTickOrderedData GetNewDataRef()
    {
        if (_count == _bufferLength)
            DoubleByteBufferLength();

        TTickOrderedData* pointer = (TTickOrderedData*)_buffer.Handle + _count;
        _count++;

        return ref *pointer;
    }

    private void DoubleByteBufferLength()
    {
        var newBufferLength = _buffer.Length << 1;

        nint newHandle = Marshal.ReAllocHGlobal(_buffer.Handle, newBufferLength);
        _buffer = new RawArray(newHandle, newBufferLength);
        _bufferLength <<= 1;
    }

    [Pure]
    public unsafe int LatestTickWithData()
    {
        if (_count == 0)
            return -1;

        TTickOrderedData* pointer = (TTickOrderedData*)_buffer.Handle + (_count - 1);

        return pointer->TickNumber;
    }

    public void Dispose()
    {
        _buffer.Dispose();
    }
}
