﻿using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class PositionHelper<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private const int AlgorithmThreshold = 10; // TODO Benchmark this and figure out the best value

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
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
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
    public LargeSimpleSet<T>.Enumerator GetAllItemsNearPosition(LevelPosition levelPosition)
    {
        if (_allTrackedItems.Count < AlgorithmThreshold)
            return _allTrackedItems.GetEnumerator();

        var chunkX = levelPosition.X >> _chunkSizeBitShift;
        var chunkY = levelPosition.Y >> _chunkSizeBitShift;

        if (chunkX < 0 || chunkX >= _numberOfHorizontalChunks ||
            chunkY < 0 || chunkY >= _numberOfVerticalChunks)
            return new LargeSimpleSet<T>.Enumerator();

        return GetEnumeratorForChunkPosition(chunkX, chunkY);
    }

    [Pure]
    public LargeSimpleSet<T>.Enumerator GetAllItemsNearRegion(
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        if (_allTrackedItems.Count < AlgorithmThreshold)
            return _allTrackedItems.GetEnumerator();

        GetShiftValues(topLeftLevelPosition, out var topLeftChunkX, out var topLeftChunkY);
        GetShiftValues(bottomRightLevelPosition, out var bottomRightChunkX, out var bottomRightChunkY);

        if (topLeftChunkX == bottomRightChunkX &&
            topLeftChunkY == bottomRightChunkY)
            return GetEnumeratorForChunkPosition(topLeftChunkX, topLeftChunkY);

        EvaluateChunkPositions(_setUnionChunkPositionUser, topLeftChunkX, topLeftChunkY, bottomRightChunkX, bottomRightChunkY);
        return _setUnionChunkPositionUser.GetEnumerator();
    }

    [Pure]
    private LargeSimpleSet<T>.Enumerator GetEnumeratorForChunkPosition(int chunkX, int chunkY)
    {
        var chunkPosition = new ChunkPosition(chunkX, chunkY);

        return _itemChunkLookup.TryGetValue(chunkPosition, out var itemChunk)
            ? itemChunk.GetEnumerator()
            : new LargeSimpleSet<T>.Enumerator();
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Item added twice!");

        if (_allTrackedItems.Count < AlgorithmThreshold)
            return;

        if (_allTrackedItems.Count > AlgorithmThreshold)
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

        if (_allTrackedItems.Count < AlgorithmThreshold)
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

    public void RemoveItem(T item)
    {
        var previousCount = _allTrackedItems.Count;
        
        if (!_allTrackedItems.Remove(item))
            throw new InvalidOperationException("Item not registered!");

        if (previousCount < AlgorithmThreshold)
            return;

        if (previousCount > AlgorithmThreshold)
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

    private void RegisterItemPosition(T item, int ax, int ay, int bx, int by)
    {
        EvaluateChunkPositions(_chunkPositionScratchSpace, ax, ay, bx, by);

        foreach (var itemChunkPosition in _chunkPositionScratchSpace.AsSpan())
        {
            if (!_itemChunkLookup.TryGetValue(itemChunkPosition, out var itemChunk))
            {
                itemChunk = new LargeSimpleSet<T>(_hasher);
                _itemChunkLookup.Add(itemChunkPosition, itemChunk);
            }

            itemChunk.Add(item);
        }
    }

    private void DeregisterItemPosition(T item, int ax, int ay, int bx, int by)
    {
        EvaluateChunkPositions(_chunkPositionScratchSpace, ax, ay, bx, by);

        foreach (var itemChunkPosition in _chunkPositionScratchSpace.AsSpan())
        {
            if (!_itemChunkLookup.TryGetValue(itemChunkPosition, out var itemChunk))
                continue;

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