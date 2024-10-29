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
    private const int SnapshotDataListSizeMultiplier = LevelConstants.InitialSnapshotDataBufferMultiplier / LevelConstants.RewindSnapshotInterval;

    private readonly TItemManager _itemManager;
    private readonly SnapshotList _snapshotList;

    public SnapshotRecorder(TItemManager itemManager)
    {
        _itemManager = itemManager;
        _snapshotList = new SnapshotList(_itemManager.NumberOfItems);
    }

    public void TakeSnapshot()
    {
        var items = _itemManager.AllItems;
        var snapshotDataSpan = _snapshotList.GetNewSnapshotDataSpan();

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
        var snapshotDataSpan = _snapshotList.GetSnapshotDataSlice(snapshotNumber);

        if (items.Length != snapshotDataSpan.Length)
            throw new InvalidOperationException("Span length mismatch!");

        for (var index = 0; index < items.Length; index++)
        {
            var item = items[index];
            ref readonly var snapshotData = ref snapshotDataSpan[index];

            item.SetFromSnapshotData(in snapshotData);
        }
    }

    private sealed class SnapshotList
    {
        private readonly int _numberOfItemsPerSnapshot;
        private TSnapshotData[] _data;
        private int _count;

        public int Count => _count;

        public SnapshotList(int numberOfItemsPerSnapshot)
        {
            _numberOfItemsPerSnapshot = numberOfItemsPerSnapshot;
            _data = new TSnapshotData[numberOfItemsPerSnapshot * SnapshotDataListSizeMultiplier];
        }

        public Span<TSnapshotData> GetNewSnapshotDataSpan()
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

        public ReadOnlySpan<TSnapshotData> GetSnapshotDataSlice(int sliceNumber)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(sliceNumber);

            _count = _numberOfItemsPerSnapshot * (1 + sliceNumber);

            return new ReadOnlySpan<TSnapshotData>(_data, sliceNumber * _numberOfItemsPerSnapshot, _numberOfItemsPerSnapshot);
        }
    }
}