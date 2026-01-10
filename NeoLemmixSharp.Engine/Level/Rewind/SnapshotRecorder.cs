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

    private bool _isDisposed;

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
        if (!_isDisposed)
        {
            _isDisposed = true;
            _buffer.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
































public sealed class SnapshotRecorder2 : IDisposable
{
    private readonly RawArray _workBuffer;
    private int _snapshotBufferCapacity;
    private RawArray _snapshotBuffer;
    private int _numberOfSnapshots;

    private bool _isDisposed;

    private int TotalNumberOfBytesPerSnapshot => _workBuffer.Length;

    public SnapshotRecorder2(RawArray workBuffer)
    {
        _workBuffer = workBuffer;

        _snapshotBufferCapacity = RewindConstants.SnapshotDataListSizeMultiplier;
        _snapshotBuffer = Helpers.AllocateBuffer<byte>(TotalNumberOfBytesPerSnapshot * RewindConstants.SnapshotDataListSizeMultiplier);
    }

    public unsafe void TakeSnapshot()
    {
        byte* snapshotDataPointer = GetNewSnapshotDataPointer();
        byte* workBufferPointer = (byte*)_workBuffer.Handle;

        var sourceSpan = Helpers.CreateReadOnlySpan<byte>(workBufferPointer, TotalNumberOfBytesPerSnapshot);
        var destinationSpan = Helpers.CreateSpan<byte>(snapshotDataPointer, TotalNumberOfBytesPerSnapshot);

        sourceSpan.CopyTo(destinationSpan);
    }

    private unsafe byte* GetNewSnapshotDataPointer()
    {
        if (_numberOfSnapshots == _snapshotBufferCapacity)
            DoubleSnapshotBufferCapacity();

        byte* pointer = (byte*)_snapshotBuffer.Handle + (_numberOfSnapshots * TotalNumberOfBytesPerSnapshot);
        _numberOfSnapshots++;

        return pointer;
    }

    private void DoubleSnapshotBufferCapacity()
    {
        var newBufferLength = _snapshotBuffer.Length * 2;

        nint newHandle = Marshal.ReAllocHGlobal(_snapshotBuffer.Handle, newBufferLength);
        _snapshotBuffer = new RawArray(newHandle, newBufferLength);
        _snapshotBufferCapacity *= 2;
    }

    public unsafe void ApplySnapshot(int snapshotNumber)
    {
        byte* snapshotDataPointer = GetDataPointerForSnapshotNumber(snapshotNumber);
        byte* workBufferPointer = (byte*)_workBuffer.Handle;

        var sourceSpan = Helpers.CreateReadOnlySpan<byte>(snapshotDataPointer, TotalNumberOfBytesPerSnapshot);
        var destinationSpan = Helpers.CreateSpan<byte>(workBufferPointer, TotalNumberOfBytesPerSnapshot);

        sourceSpan.CopyTo(destinationSpan);
    }

    private unsafe byte* GetDataPointerForSnapshotNumber(int snapshotNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(snapshotNumber);

        _numberOfSnapshots = snapshotNumber + 1;

        byte* pointer = (byte*)_snapshotBuffer.Handle + (snapshotNumber * TotalNumberOfBytesPerSnapshot);
        return pointer;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            _snapshotBuffer.Dispose();
            _workBuffer.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
