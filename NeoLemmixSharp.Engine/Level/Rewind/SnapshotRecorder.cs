using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class SnapshotRecorder<TItemManager, TItemType> : IDisposable
    where TItemManager : class, IItemManager<TItemType>
    where TItemType : class, ISnapshotDataConvertible
{
    private readonly TItemManager _itemManager;
    private RawArray _buffer;
    private readonly int _requiredNumberOfBytesPerSnapshot;
    private int _snapshotBufferCapacity;
    private int _numberOfSnapshots;

    public SnapshotRecorder(TItemManager itemManager)
    {
        _itemManager = itemManager;

        var requiredNumberOfBytesPerSnapshotTotal = CalculateRequiredNumberOfBytesForSnapshotting();

        _requiredNumberOfBytesPerSnapshot = requiredNumberOfBytesPerSnapshotTotal;
        _snapshotBufferCapacity = RewindConstants.SnapshotDataListSizeMultiplier;
        _buffer = Helpers.AllocateBuffer<byte>(requiredNumberOfBytesPerSnapshotTotal * RewindConstants.SnapshotDataListSizeMultiplier);
    }

    private int CalculateRequiredNumberOfBytesForSnapshotting()
    {
        var requiredNumberOfBytesPerSnapshotTotal = 0;

        foreach (var item in _itemManager.AllItems)
        {
            requiredNumberOfBytesPerSnapshotTotal += item.GetRequiredNumberOfBytesForSnapshotting();
        }

        return requiredNumberOfBytesPerSnapshotTotal;
    }

    public unsafe void TakeSnapshot()
    {
        byte* snapshotDataPointer = GetNewSnapshotDataPointer();
        var pointerOffset = 0;

        foreach (var item in _itemManager.AllItems)
        {
            byte* p = snapshotDataPointer + pointerOffset;
            item.WriteToSnapshotData(p);

            var pointerIncrement = item.GetRequiredNumberOfBytesForSnapshotting();
            pointerOffset += pointerIncrement;
        }
    }

    private unsafe byte* GetNewSnapshotDataPointer()
    {
        if (_numberOfSnapshots == _snapshotBufferCapacity)
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
        _snapshotBufferCapacity <<= 1;
    }

    public unsafe void ApplySnapshot(int snapshotNumber)
    {
        byte* snapshotDataPointer = GetDataPointerForSnapshotNumber(snapshotNumber);
        var pointerOffset = 0;

        foreach (var item in _itemManager.AllItems)
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
