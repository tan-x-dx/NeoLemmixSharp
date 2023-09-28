using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class SpacialHashGrid<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private readonly int _chunkSizeBitShift;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly ISimpleHasher<T> _hasher;
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly SimpleSet<T> _allTrackedItems;
    private readonly SimpleSet<T> _setUnionScratchSpace;

    private readonly SimpleSet<T>?[] _itemChunkLookup;

    public SpacialHashGrid(
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

        _allTrackedItems = new SimpleSet<T>(hasher);
        _setUnionScratchSpace = new SimpleSet<T>(hasher);

        _itemChunkLookup = new SimpleSet<T>?[_numberOfHorizontalChunks * _numberOfVerticalChunks];
    }

    [Pure]
    public bool IsTrackingItem(T item) => _allTrackedItems.Contains(item);

    [Pure]
    public SimpleSet<T> GetAllTrackedItems() => _allTrackedItems;

    [Pure]
    public SimpleSet<T> GetAllItemsNearPosition(LevelPosition levelPosition)
    {
        if (_allTrackedItems.Count == 0)
            return SimpleSet<T>.Empty;

        var chunkX = levelPosition.X >> _chunkSizeBitShift;
        var chunkY = levelPosition.Y >> _chunkSizeBitShift;

        if (chunkX < 0 || chunkX >= _numberOfHorizontalChunks ||
            chunkY < 0 || chunkY >= _numberOfVerticalChunks)
            return SimpleSet<T>.Empty;

        return GetItemsForChunkPosition(chunkX, chunkY);
    }

    [Pure]
    public SimpleSet<T> GetAllItemsNearRegion(LevelPositionPair levelRegion)
    {
        if (_allTrackedItems.Count == 0)
            return SimpleSet<T>.Empty;

        var topLeftLevelPosition = levelRegion.GetTopLeftPosition();
        var bottomRightLevelPosition = levelRegion.GetBottomRightPosition();

        GetShiftValues(topLeftLevelPosition, out var topLeftChunkX, out var topLeftChunkY);
        GetShiftValues(bottomRightLevelPosition, out var bottomRightChunkX, out var bottomRightChunkY);

        if (topLeftChunkX == bottomRightChunkX &&
            topLeftChunkY == bottomRightChunkY)
            return GetItemsForChunkPosition(topLeftChunkX, topLeftChunkY);

        _setUnionScratchSpace.Clear();
        EvaluateChunkPositions(UseChunkPositionMode.Union, null, topLeftChunkX, topLeftChunkY, bottomRightChunkX, bottomRightChunkY);
        return _setUnionScratchSpace;
    }

    [Pure]
    private SimpleSet<T> GetItemsForChunkPosition(int chunkX, int chunkY)
    {
        var index = _numberOfHorizontalChunks * chunkY + chunkX;
        var itemChunk = _itemChunkLookup[index];
        return itemChunk ?? SimpleSet<T>.Empty;
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Item added twice!");

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

            AddToChunk(item, ax, ay);

            return;
        }

        EvaluateChunkPositions(UseChunkPositionMode.Add, item, ax, ay, bx, by);
    }

    private void AddToChunk(T item, int x, int y)
    {
        var index = _numberOfHorizontalChunks * y + x;
        ref var itemChunk = ref _itemChunkLookup[index];
        itemChunk ??= new SimpleSet<T>(_hasher);

        itemChunk.Add(item);
    }

    public void RemoveItem(T item)
    {
        if (!_allTrackedItems.Remove(item))
            throw new InvalidOperationException("Item not registered!");

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

            RemoveFromChunk(item, ax, ay);

            return;
        }

        EvaluateChunkPositions(UseChunkPositionMode.Remove, item, ax, ay, bx, by);
    }

    private void RemoveFromChunk(T item, int x, int y)
    {
        var index = _numberOfHorizontalChunks * y + x;
        var itemChunk = _itemChunkLookup[index];

        itemChunk?.Remove(item);
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

    private void EvaluateChunkPositions(UseChunkPositionMode useChunkPositionMode, T? item, int ax, int ay, int bx, int by)
    {
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
                UseChunkPosition(useChunkPositionMode, item, x1, y1);

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

    private void UseChunkPosition(UseChunkPositionMode useChunkPositionMode, T? item, int x, int y)
    {
        if (useChunkPositionMode == UseChunkPositionMode.Add)
        {
            AddToChunk(item!, x, y);
            return;
        }

        if (useChunkPositionMode == UseChunkPositionMode.Remove)
        {
            RemoveFromChunk(item!, x, y);
            return;
        }

        if (useChunkPositionMode == UseChunkPositionMode.Union)
        {
            var index = _numberOfHorizontalChunks * y + x;
            var itemChunk = _itemChunkLookup[index];

            if (itemChunk is null)
                return;
            _setUnionScratchSpace.UnionWith(itemChunk);


            return;
        }

        throw new ArgumentOutOfRangeException(nameof(useChunkPositionMode), useChunkPositionMode, "Invalid value");
    }

    private enum UseChunkPositionMode
    {
        Add,
        Remove,
        Union
    }
}