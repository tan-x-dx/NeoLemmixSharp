using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class SnapshotManager<TPerfectHasher, TItemType, TSnapshotData>
    where TPerfectHasher : IItemManager<TItemType>
    where TItemType : class, ISnapshotDataConvertible<TSnapshotData>
    where TSnapshotData : struct
{
    private const int InitialSnapshotDataListSize = 1 << 14;

    private readonly TPerfectHasher _hasher;
    private readonly SnapshotList _snapshotList;

    public SnapshotManager(TPerfectHasher hasher)
    {
        _hasher = hasher;
        _snapshotList = new SnapshotList(_hasher.NumberOfItems);
    }

    public void TakeSnapshot()
    {
        var items = _hasher.AllItems;
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

    private sealed class SnapshotList
    {
        private readonly int _numberOfItems;
        private TSnapshotData[] _data;
        private int _count;

        public int Count => _count;

        public SnapshotList(int numberOfItems)
        {
            _numberOfItems = numberOfItems;
            _data = new TSnapshotData[numberOfItems * 16];
        }

        public ReadOnlySpan<TSnapshotData> Slice(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Negative start index");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Negative length");
            if (_count - start < length)
                throw new ArgumentOutOfRangeException(nameof(start), "Start index with length is out of bounds");

            return new ReadOnlySpan<TSnapshotData>(_data, start, length);
        }

        public ReadOnlySpan<TSnapshotData> SliceToEnd(int start)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Negative start index");

            return new ReadOnlySpan<TSnapshotData>(_data, start, Math.Max(0, _count - start));
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

            _count += _numberOfItems;

            return new Span<TSnapshotData>(_data, count, _numberOfItems);
        }
    }
}