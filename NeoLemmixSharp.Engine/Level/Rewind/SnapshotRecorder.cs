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
        _numberOfItemsPerSnapshot = itemManager.AllItems.Length;
        _bufferLength = _numberOfItemsPerSnapshot * SnapshotDataListSizeMultiplier;
        _buffer = Helpers.CreateBuffer<TSnapshotData>(_bufferLength);
    }

    public void TakeSnapshot()
    {
        var items = _itemManager.AllItems;
        var snapshotDataSpan = GetNewSnapshotDataSpan();

        if (items.Length != snapshotDataSpan.Length)
            throw new InvalidOperationException("Span length mismatch!");

        for (var index = 0; index < items.Length; index++)
        {
            var item = items[index];
            ref var snapshotData = ref snapshotDataSpan[index];

            item.WriteToSnapshotData(out snapshotData);
        }
    }

    public void ApplySnapshot(int snapshotNumber)
    {
        var items = _itemManager.AllItems;
        var snapshotDataSpan = GetSnapshotDataSlice(snapshotNumber);

        if (items.Length != snapshotDataSpan.Length)
            throw new InvalidOperationException("Span length mismatch!");

        for (var index = 0; index < items.Length; index++)
        {
            var item = items[index];
            ref readonly var snapshotData = ref snapshotDataSpan[index];

            item.SetFromSnapshotData(in snapshotData);
        }
    }

    private unsafe Span<TSnapshotData> GetNewSnapshotDataSpan()
    {
        if (_count == _bufferLength)
            DoubleByteBufferLength();

        TSnapshotData* pointer = (TSnapshotData*)_buffer.Handle + _count;
        _count += _numberOfItemsPerSnapshot;

        return new Span<TSnapshotData>(pointer, _numberOfItemsPerSnapshot);
    }

    private void DoubleByteBufferLength()
    {
        var newBufferLength = _buffer.Length << 1;

        nint newHandle = Marshal.ReAllocHGlobal(_buffer.Handle, newBufferLength);
        _buffer = new RawArray(newHandle, newBufferLength);
        _bufferLength <<= 1;
    }

    private unsafe ReadOnlySpan<TSnapshotData> GetSnapshotDataSlice(int sliceNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sliceNumber);

        _count = _numberOfItemsPerSnapshot * (1 + sliceNumber);

        TSnapshotData* pointer = (TSnapshotData*)_buffer.Handle + (sliceNumber * _numberOfItemsPerSnapshot);
        return new ReadOnlySpan<TSnapshotData>(pointer, _numberOfItemsPerSnapshot);
    }

    public void Dispose()
    {
        _buffer.Dispose();
    }
}
