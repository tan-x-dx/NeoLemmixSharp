using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class PositionHelper<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private const int AlgorithmThreshold = 10; // TODO Benchmark this and figure out the best value

    private readonly int _algorithmThreshold;

    private readonly int _chunkSizeBitShift;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly ISimpleHasher<T> _hasher;
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly LargeSimpleSet<T> _allTrackedItems;

    private readonly Dictionary<ChunkPosition, LargeSimpleSet<T>> _itemChunkLookup;
    private readonly SimpleChunkPositionList _chunkPositionScratchSpace;
    private readonly SetUnionChunkPositionUser<T> _setUnionChunkPositionUser;

    public PositionHelper(
        ISimpleHasher<T> hasher,
        ChunkSizeType chunkSizeType,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour,
        int algorithmThreshold = AlgorithmThreshold)
    {
        _algorithmThreshold = algorithmThreshold;

        _hasher = hasher;
        _chunkSizeBitShift = chunkSizeType.ChunkSizeBitShiftFromType();
        var chunkSize = 1 << _chunkSizeBitShift;
        var chunkSizeBitMask = chunkSize - 1;

        _numberOfHorizontalChunks = (horizontalBoundaryBehaviour.LevelWidth + chunkSizeBitMask) >> _chunkSizeBitShift;
        _numberOfVerticalChunks = (verticalBoundaryBehaviour.LevelHeight + chunkSizeBitMask) >> _chunkSizeBitShift;

        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _allTrackedItems = new LargeSimpleSet<T>(hasher);

        _itemChunkLookup = new Dictionary<ChunkPosition, LargeSimpleSet<T>>(ChunkPositionEqualityComparer.Instance);
        _chunkPositionScratchSpace = new SimpleChunkPositionList();
        _setUnionChunkPositionUser = new SetUnionChunkPositionUser<T>(hasher, _itemChunkLookup);
    }

    [Pure]
    public bool IsTrackingItem(T item) => _allTrackedItems.Contains(item);

    [Pure]
    public LargeSimpleSet<T> GetAllTrackedItemsEnumerator() => _allTrackedItems;

    [Pure]
    public LargeSimpleSet<T> GetAllItemsNearPosition(LevelPosition levelPosition)
    {
        if (_allTrackedItems.Count < _algorithmThreshold)
            return _allTrackedItems;

        var chunkX = levelPosition.X >> _chunkSizeBitShift;
        var chunkY = levelPosition.Y >> _chunkSizeBitShift;

        if (chunkX < 0 || chunkX >= _numberOfHorizontalChunks ||
            chunkY < 0 || chunkY >= _numberOfVerticalChunks)
            return LargeSimpleSet<T>.Empty;

        return GetEnumeratorForChunkPosition(chunkX, chunkY);
    }

    [Pure]
    public LargeSimpleSet<T> GetAllItemsNearRegion(
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        if (_allTrackedItems.Count < _algorithmThreshold)
            return _allTrackedItems;

        GetShiftValues(topLeftLevelPosition, out var topLeftChunkX, out var topLeftChunkY);
        GetShiftValues(bottomRightLevelPosition, out var bottomRightChunkX, out var bottomRightChunkY);

        if (topLeftChunkX == bottomRightChunkX &&
            topLeftChunkY == bottomRightChunkY)
            return GetEnumeratorForChunkPosition(topLeftChunkX, topLeftChunkY);

        EvaluateChunkPositions(_setUnionChunkPositionUser, topLeftChunkX, topLeftChunkY, bottomRightChunkX, bottomRightChunkY);
        return _setUnionChunkPositionUser.GetSet();
    }

    [Pure]
    private LargeSimpleSet<T> GetEnumeratorForChunkPosition(int chunkX, int chunkY)
    {
        var chunkPosition = new ChunkPosition(chunkX, chunkY);

        ref var itemChunk = ref CollectionsMarshal.GetValueRefOrNullRef(_itemChunkLookup, chunkPosition);
        return Unsafe.IsNullRef(ref itemChunk)
            ? LargeSimpleSet<T>.Empty
            : itemChunk;
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Item added twice!");

        if (_allTrackedItems.Count < _algorithmThreshold)
            return;

        if (_allTrackedItems.Count > _algorithmThreshold)
        {
            StartTrackingItemPosition(item);
            return;
        }

        foreach (var trackedItem in _allTrackedItems)
        {
            StartTrackingItemPosition(trackedItem);
        }
    }

    private void StartTrackingItemPosition(T item)
    {
        var topLeftPixel = item.TopLeftPixel;
        var bottomRightPixel = item.BottomRightPixel;

        GetShiftValues(topLeftPixel, out var topLeftChunkX, out var topLeftChunkY);
        GetShiftValues(bottomRightPixel, out var bottomRightChunkX, out var bottomRightChunkY);

        RegisterItemPosition(item, topLeftChunkX, topLeftChunkY, bottomRightChunkX, bottomRightChunkY);
    }

    public void UpdateItemPosition(T item)
    {
        if (!_allTrackedItems.Contains(item))
            throw new InvalidOperationException("Item not registered!");

        if (_allTrackedItems.Count < _algorithmThreshold)
            return;

        var topLeftPixel = item.TopLeftPixel;
        var bottomRightPixel = item.BottomRightPixel;
        var previousTopLeftPixel = item.PreviousTopLeftPixel;
        var previousBottomRightPixel = item.PreviousBottomRightPixel;

        if (topLeftPixel == previousTopLeftPixel &&
            bottomRightPixel == previousBottomRightPixel)
            return;

        GetShiftValues(topLeftPixel, out var topLeftChunkX, out var topLeftChunkY);
        GetShiftValues(bottomRightPixel, out var bottomRightChunkX, out var bottomRightChunkY);

        GetShiftValues(previousTopLeftPixel, out var previousTopLeftChunkX, out var previousTopLeftChunkY);
        GetShiftValues(previousBottomRightPixel, out var previousBottomRightChunkX, out var previousBottomRightChunkY);

        if (topLeftChunkX == previousTopLeftChunkX &&
            topLeftChunkY == previousTopLeftChunkY &&
            bottomRightChunkX == previousBottomRightChunkX &&
            bottomRightChunkY == previousBottomRightChunkY)
            return;

        DeregisterItemPosition(item, previousTopLeftChunkX, previousTopLeftChunkY, previousBottomRightChunkX, previousBottomRightChunkY);
        RegisterItemPosition(item, topLeftChunkX, topLeftChunkY, bottomRightChunkX, bottomRightChunkY);
    }

    private void RegisterItemPosition(T item, int ax, int ay, int bx, int by)
    {
        if (ax == bx &&
            ay == by)
        {
            // Only one chunk -> skip some extra work

            var chunkPosition = new ChunkPosition(ax, ay);
            AddToChunk(item, chunkPosition);

            return;
        }

        EvaluateChunkPositions(_chunkPositionScratchSpace, ax, ay, bx, by);

        foreach (var chunkPosition in _chunkPositionScratchSpace.AsSpan())
        {
            AddToChunk(item, chunkPosition);
        }
    }

    private void AddToChunk(T item, ChunkPosition chunkPosition)
    {
        ref var itemChunk = ref CollectionsMarshal.GetValueRefOrAddDefault(_itemChunkLookup, chunkPosition, out var exists);
        if (!exists)
        {
            itemChunk = new LargeSimpleSet<T>(_hasher);
        }

        itemChunk!.Add(item);
    }

    public void RemoveItem(T item)
    {
        var previousCount = _allTrackedItems.Count;

        if (!_allTrackedItems.Remove(item))
            throw new InvalidOperationException("Item not registered!");

        if (previousCount < _algorithmThreshold)
            return;

        if (previousCount > _algorithmThreshold)
        {
            StopTrackingItemPosition(item);
            return;
        }

        foreach (var trackedItem in _allTrackedItems)
        {
            StopTrackingItemPosition(trackedItem);
        }
    }

    private void StopTrackingItemPosition(T item)
    {
        var topLeftPixel = item.TopLeftPixel;
        var bottomRightPixel = item.BottomRightPixel;
        var previousTopLeftPixel = item.PreviousTopLeftPixel;
        var previousBottomRightPixel = item.PreviousBottomRightPixel;

        GetShiftValues(topLeftPixel, out var topLeftChunkX, out var topLeftChunkY);
        GetShiftValues(bottomRightPixel, out var bottomRightChunkX, out var bottomRightChunkY);

        DeregisterItemPosition(item, topLeftChunkX, topLeftChunkY, bottomRightChunkX, bottomRightChunkY);

        GetShiftValues(previousTopLeftPixel, out var previousTopLeftChunkX, out var previousTopLeftChunkY);
        GetShiftValues(previousBottomRightPixel, out var previousBottomRightChunkX, out var previousBottomRightChunkY);

        if (topLeftChunkX == previousTopLeftChunkX &&
            topLeftChunkY == previousTopLeftChunkY &&
            bottomRightChunkX == previousBottomRightChunkX &&
            bottomRightChunkY == previousBottomRightChunkY)
            return;

        DeregisterItemPosition(item, previousTopLeftChunkX, previousTopLeftChunkY, previousBottomRightChunkX, previousBottomRightChunkY);
    }

    private void DeregisterItemPosition(T item, int ax, int ay, int bx, int by)
    {
        if (ax == bx &&
            ay == by)
        {
            // Only one chunk -> skip some extra work

            var chunkPosition = new ChunkPosition(ax, ay);
            RemoveFromChunk(item, chunkPosition);

            return;
        }

        EvaluateChunkPositions(_chunkPositionScratchSpace, ax, ay, bx, by);

        foreach (var chunkPosition in _chunkPositionScratchSpace.AsSpan())
        {
            RemoveFromChunk(item, chunkPosition);
        }
    }

    private void RemoveFromChunk(T item, ChunkPosition chunkPosition)
    {
        ref var itemChunk = ref CollectionsMarshal.GetValueRefOrNullRef(_itemChunkLookup, chunkPosition);
        if (!Unsafe.IsNullRef(ref itemChunk))
        {
            itemChunk.Remove(item);
        }
    }

    private void GetShiftValues(
        LevelPosition levelPosition,
        out int chunkX,
        out int chunkY)
    {
        chunkX = _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X) >> _chunkSizeBitShift;
        chunkY = _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y) >> _chunkSizeBitShift;
        chunkX = Math.Clamp(chunkX, 0, _numberOfHorizontalChunks - 1);
        chunkY = Math.Clamp(chunkY, 0, _numberOfVerticalChunks - 1);
    }

    private void EvaluateChunkPositions(IChunkPositionUser chunkPositionUser, int ax, int ay, int bx, int by)
    {
        chunkPositionUser.Clear();

        if (bx < ax)
        {
            bx += _numberOfHorizontalChunks;
        }
        var x = 1 + bx - ax;

        if (by < ay)
        {
            by += _numberOfVerticalChunks;
        }
        var yCount = 1 + by - ay;

        var x1 = ax;
        while (x-- > 0)
        {
            var y1 = ay;
            var y = yCount;
            while (y-- > 0)
            {
                var chunkPosition = new ChunkPosition(x1, y1);

                chunkPositionUser.UseChunkPosition(chunkPosition);

                if (++y1 == _numberOfVerticalChunks)
                {
                    y1 = 0;
                }
            }

            if (++x1 == _numberOfHorizontalChunks)
            {
                x1 = 0;
            }
        }
    }
}