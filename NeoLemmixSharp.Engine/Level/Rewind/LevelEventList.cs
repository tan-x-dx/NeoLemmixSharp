using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class LevelEventList<TEventData> : IDisposable
    where TEventData : unmanaged, ILevelEventData
{
    private RawArray _buffer;
    private int _bufferLength;
    private int _count;
    private bool _isDisposed;

    public LevelEventList(int initialCapacity)
    {
        _bufferLength = initialCapacity;
        _buffer = Helpers.AllocateBuffer<TEventData>(_bufferLength);
    }

    public int Count => _count;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe ReadOnlySpan<TEventData> GetReadOnlySpan(int start, int length)
    {
        TEventData* pointer = (TEventData*)_buffer.Handle + start;

        return Helpers.CreateReadOnlySpan<TEventData>(pointer, length);
    }

    [Pure]
    public ReadOnlySpan<TEventData> Slice(int start, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        if (_count - start < length)
            throw new ArgumentOutOfRangeException(nameof(start), "Start index with length is out of bounds");

        return GetReadOnlySpan(start, length);
    }

    [Pure]
    public ReadOnlySpan<TEventData> GetSliceToEnd(int start)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);

        return GetReadOnlySpan(start, Math.Max(0, _count - start));
    }

    public ReadOnlySpan<TEventData> RewindBackTo(int tick)
    {
        var index = 0;

        if (_count > 0)
            TryGetSmallestIndexOfTick(tick, out index);

        var result = GetSliceToEnd(index);

        _count = index;

        return result;
    }

    [Pure]
    public unsafe bool TryGetDataForTick(int tick, out TEventData* dataPointer)
    {
        if (_count > 0 && TryGetSmallestIndexOfTick(tick, out var index))
        {
            dataPointer = (TEventData*)_buffer.Handle + index;
            return true;
        }

        dataPointer = default;
        return false;
    }

    /// <summary>
    /// Finds the smallest index such that the data at that index has a TickNumber equal to or exceeding the input parameter.
    /// Returns <see langword="true" /> if the resulting index has a TickNumber exactly equal to the input parameter, <see langword="false" /> otherwise.
    /// <para>
    /// Binary search algorithm - O(log n).
    /// </para>
    /// </summary>
    /// <param name="tick">The required TickNumber</param>
    /// <param name="index">The smallest index such that the data at that index has a TickNumber equal to or exceeding the input parameter.</param>
    /// <returns><see langword="true" /> if the resulting index has a TickNumber exactly equal to the input parameter, <see langword="false" /> otherwise.</returns>
    /// <remarks>If ALL items have a TickNumber less than the input parameter, then the out index variable will be set to the array's current length.
    /// If the array is empty, then the out index variable will be set to -1.
    /// These values are out of bounds! Don't forget about this!</remarks>
    private unsafe bool TryGetSmallestIndexOfTick(int tick, out int index)
    {
        if (_count == 0)
        {
            // This is deliberately outside the bounds of the array
            // Subsequent usages of this data must deal with it
            index = -1;
            return false;
        }

        TEventData* p = (TEventData*)_buffer.Handle;
        // Edge case: All items are >= tick
        if (p->TickNumber >= tick)
        {
            index = 0;
            return p->TickNumber == tick;
        }

        p += _count;
        p--;

        // Edge case: All items are < tick
        // Also takes care of the edge case when the array is of length 1
        if (p->TickNumber < tick)
        {
            // This is deliberately outside the bounds of the array
            // Subsequent usages of this data must deal with it
            index = _count;
            return false;
        }

        return SearchForSmallestIndexOfTick(tick, out index);

        bool SearchForSmallestIndexOfTick(int tickValue, out int requiredIndex)
        {
            TEventData* basePointer = (TEventData*)_buffer.Handle;
            uint lowerTestIndex = 0;
            uint upperTestIndex = (uint)_count - 1;

            while (upperTestIndex - lowerTestIndex > 1)
            {
                uint bestGuessIndex = (lowerTestIndex + upperTestIndex) >>> 1;
                TEventData* pointer = basePointer + bestGuessIndex;

                if (pointer->TickNumber >= tickValue)
                {
                    upperTestIndex = bestGuessIndex;
                }
                else
                {
                    lowerTestIndex = bestGuessIndex;
                }
            }

            requiredIndex = (int)upperTestIndex;
            return (basePointer + upperTestIndex)->TickNumber == tickValue;
        }
    }

    public unsafe TEventData* GetNewDataPointer()
    {
        if (_count == _bufferLength)
            DoubleByteBufferLength();

        TEventData* pointer = (TEventData*)_buffer.Handle + _count;
        _count++;

        return pointer;
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
        var c = _count;
        if (c == 0)
            return -1;

        c--;
        TEventData* pointer = (TEventData*)_buffer.Handle + c;

        return pointer->TickNumber;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            _buffer.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
