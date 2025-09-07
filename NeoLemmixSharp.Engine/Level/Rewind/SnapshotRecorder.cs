﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class SnapshotRecorder<TItemManager, TItemType> : IDisposable
    where TItemManager : IItemManager<TItemType>
    where TItemType : class, ISnapshotDataConvertible
{
    private readonly TItemManager _itemManager;
    private RawArray _buffer;
    private readonly int _requiredNumberOfBytesPerSnapshot;
    private int _bufferSnapshotCapacity;
    private int _numberOfSnapshots;

    public SnapshotRecorder(TItemManager itemManager)
    {
        _itemManager = itemManager;

        var allItems = _itemManager.AllItems;
        var requiredNumberOfBytesPerSnapshotTotal = 0;

        foreach (var item in allItems)
        {
            requiredNumberOfBytesPerSnapshotTotal += item.GetRequiredNumberOfBytesForSnapshotting();
        }

        _requiredNumberOfBytesPerSnapshot = requiredNumberOfBytesPerSnapshotTotal;
        _bufferSnapshotCapacity = RewindConstants.SnapshotDataListSizeMultiplier;
        _buffer = Helpers.AllocateBuffer<byte>(requiredNumberOfBytesPerSnapshotTotal * RewindConstants.SnapshotDataListSizeMultiplier);
    }

    public unsafe void TakeSnapshot()
    {
        var items = _itemManager.AllItems;
        byte* snapshotDataPointer = GetNewSnapshotDataPointer();
        var pointerOffset = 0;

        foreach (var item in items)
        {
            byte* p = snapshotDataPointer + pointerOffset;
            item.WriteToSnapshotData(p);

            var pointerIncrement = item.GetRequiredNumberOfBytesForSnapshotting();
            pointerOffset += pointerIncrement;
        }
    }

    private unsafe byte* GetNewSnapshotDataPointer()
    {
        if (_numberOfSnapshots == _bufferSnapshotCapacity)
            DoubleSnapshotBufferCapacity();

        byte* pointer = (byte*)_buffer.Handle + (_numberOfSnapshots * _requiredNumberOfBytesPerSnapshot);
        _numberOfSnapshots++;

        return pointer;
    }

    private void DoubleSnapshotBufferCapacity()
    {
        var newBufferLength = _buffer.Length << 1;

        nint newHandle = Marshal.ReAllocHGlobal(_buffer.Handle, newBufferLength);
        _buffer = new RawArray(newHandle, newBufferLength);
        _bufferSnapshotCapacity <<= 1;
    }

    public unsafe void ApplySnapshot(int snapshotNumber)
    {
        var items = _itemManager.AllItems;
        byte* snapshotDataPointer = GetDataPointerForSnapshotNumber(snapshotNumber);
        var pointerOffset = 0;

        foreach (var item in items)
        {
            byte* p = snapshotDataPointer + pointerOffset;
            item.SetFromSnapshotData(p);

            var pointerIncrement = item.GetRequiredNumberOfBytesForSnapshotting();
            pointerOffset += pointerIncrement;
        }
    }

    private unsafe byte* GetDataPointerForSnapshotNumber(int snapshotNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(snapshotNumber);

        _numberOfSnapshots = snapshotNumber + 1;

        byte* pointer = (byte*)_buffer.Handle + (snapshotNumber * _requiredNumberOfBytesPerSnapshot);
        return pointer;
    }

    public void Dispose()
    {
        _buffer.Dispose();
    }
}
