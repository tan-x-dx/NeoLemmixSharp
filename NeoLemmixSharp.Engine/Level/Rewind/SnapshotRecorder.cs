using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class SnapshotRecorder<TItemManager, TItemType, TSnapshotData>
    where TItemManager : IItemManager<TItemType>
    where TItemType : class, ISnapshotDataConvertible<TSnapshotData>
    where TSnapshotData : unmanaged
{
    /// <summary>
    /// Allocate enough space initially for four minutes of gameplay.
    /// If gameplay lasts longer, then the list will double in capacity.
    /// </summary>
    private const int SnapshotDataListSizeMultiplier = EngineConstants.InitialSnapshotDataBufferMultiplier / EngineConstants.RewindSnapshotInterval;

    private readonly TItemManager _itemManager;
    private readonly int _numberOfItemsPerSnapshot;
    private TSnapshotData[] _data;
    private int _count;

    public int Count => _count;

    public SnapshotRecorder(TItemManager itemManager)
    {
        _itemManager = itemManager;
        _numberOfItemsPerSnapshot = itemManager.NumberOfItems;
        _data = new TSnapshotData[_numberOfItemsPerSnapshot * SnapshotDataListSizeMultiplier];
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

            item.ToSnapshotData(out snapshotData);
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

    private Span<TSnapshotData> GetNewSnapshotDataSpan()
    {
        var arraySize = _data.Length;
        var count = _count;
        if (count == arraySize)
        {
            var newArray = new TSnapshotData[arraySize * 2];
            new ReadOnlySpan<TSnapshotData>(_data).CopyTo(new Span<TSnapshotData>(newArray));

            _data = newArray;
        }

        _count += _numberOfItemsPerSnapshot;

        return new Span<TSnapshotData>(_data, count, _numberOfItemsPerSnapshot);
    }

    private ReadOnlySpan<TSnapshotData> GetSnapshotDataSlice(int sliceNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sliceNumber);

        _count = _numberOfItemsPerSnapshot * (1 + sliceNumber);

        return new ReadOnlySpan<TSnapshotData>(_data, sliceNumber * _numberOfItemsPerSnapshot, _numberOfItemsPerSnapshot);
    }
}