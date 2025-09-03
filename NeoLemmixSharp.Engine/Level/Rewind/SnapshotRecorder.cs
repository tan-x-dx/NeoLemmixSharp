using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class SnapshotRecorder<TItemManager, TItemType, TSnapshotData> : IDisposable
    where TItemManager : IItemManager<TItemType>
    where TItemType : class, ISnapshotDataConvertible<TSnapshotData>
    where TSnapshotData : unmanaged
{
    /// <summary>
    /// Allocate enough space initially for four minutes of gameplay.
    /// If gameplay lasts longer, then the buffer will double in capacity.
    /// </summary>
    private const int SnapshotDataListSizeMultiplier = EngineConstants.InitialSnapshotDataBufferMultiplier / EngineConstants.RewindSnapshotInterval;

    private readonly TItemManager _itemManager;
    private readonly int _numberOfItemsPerSnapshot;
    private RawArray _buffer;
    private int _bufferLength;
    private int _count;

    public int Count => _count;

    public SnapshotRecorder(TItemManager itemManager)
    {
        _itemManager = itemManager;

        var allItems = _itemManager.AllItems;
        var requiredNumberOfBytesPerSnapshotTotal = 0;

        foreach (var item in allItems)
        {
            requiredNumberOfBytesPerSnapshotTotal += item.GetRequiredNumberOfBytesForSnapshotting();
        }

        _numberOfItemsPerSnapshot = allItems.Length;
        _bufferLength = requiredNumberOfBytesPerSnapshotTotal * SnapshotDataListSizeMultiplier;
        _buffer = Helpers.AllocateBuffer<TSnapshotData>(_bufferLength);
    }

    public unsafe void TakeSnapshot()
    {
        var items = _itemManager.AllItems;
        TSnapshotData* snapshotDataPointer = GetLatestSnapshotDataPointer();
        var pointerOffset = 0;

        foreach (var item in items)
        {
            TSnapshotData* p = snapshotDataPointer + pointerOffset;

            var pointerIncrement = item.WriteToSnapshotData(p);
            pointerOffset += pointerIncrement;
        }
    }

    public unsafe void ApplySnapshot(int snapshotNumber)
    {
        var items = _itemManager.AllItems;
        TSnapshotData* snapshotDataPointer = GetSnapshotDataPointerForSlice(snapshotNumber);
        var pointerOffset = 0;

        foreach (var item in items)
        {
            TSnapshotData* p = snapshotDataPointer + pointerOffset;

            var pointerIncrement = item.SetFromSnapshotData(p);
            pointerOffset += pointerIncrement;
        }
    }

    private unsafe TSnapshotData* GetLatestSnapshotDataPointer()
    {
        if (_count == _bufferLength)
            DoubleByteBufferLength();

        TSnapshotData* pointer = (TSnapshotData*)_buffer.Handle + _count;
        _count += _numberOfItemsPerSnapshot;

        return pointer;
    }

    private void DoubleByteBufferLength()
    {
        var newBufferLength = _buffer.Length << 1;

        nint newHandle = Marshal.ReAllocHGlobal(_buffer.Handle, newBufferLength);
        _buffer = new RawArray(newHandle, newBufferLength);
        _bufferLength <<= 1;
    }

    private unsafe TSnapshotData* GetSnapshotDataPointerForSlice(int sliceNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sliceNumber);

        _count = _numberOfItemsPerSnapshot * (1 + sliceNumber);

        TSnapshotData* pointer = (TSnapshotData*)_buffer.Handle + (sliceNumber * _numberOfItemsPerSnapshot);
        return pointer;
    }

    public void Dispose()
    {
        _buffer.Dispose();
    }
}
