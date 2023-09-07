using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class PositionHelper<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private readonly int _chunkSizeBitShift;

    private readonly T[] _allItems;
    private readonly ISimpleHasher<T> _hasher;

    private readonly Dictionary<ChunkPosition, LargeSimpleSet<T>> _itemChunkLookup;

    private readonly SimpleChunkPositionList _chunkPositionScratchSpaceAdd;
    private readonly SimpleChunkPositionList _chunksPositionScratchSpaceRemove;

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly SetUnionChunkPositionUser<T> _setUnionChunkPositionUser;

    internal Dictionary<ChunkPosition, LargeSimpleSet<T>> ItemChunks => _itemChunkLookup;

    public PositionHelper(
        T[] allItems,
        ISimpleHasher<T> hasher,
        ChunkSizeType chunkSizeType,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allItems = allItems;
        _hasher = hasher;

        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _chunkSizeBitShift = chunkSizeType.ChunkSizeBitShiftFromType();
        var chunkSize = 1 << _chunkSizeBitShift;
        var chunkSizeBitMask = chunkSize - 1;

        _numberOfHorizontalChunks = (_horizontalBoundaryBehaviour.LevelWidth + chunkSizeBitMask) >> _chunkSizeBitShift;
        _numberOfVerticalChunks = (_verticalBoundaryBehaviour.LevelHeight + chunkSizeBitMask) >> _chunkSizeBitShift;

        _itemChunkLookup = new Dictionary<ChunkPosition, LargeSimpleSet<T>>(ChunkPositionEqualityComparer.Instance);

        _chunkPositionScratchSpaceAdd = new SimpleChunkPositionList();
        _chunksPositionScratchSpaceRemove = new SimpleChunkPositionList();

        _setUnionChunkPositionUser = new SetUnionChunkPositionUser<T>(this);
    }

    [Pure]
    public LargeSimpleSet<T>.Enumerator GetAllItemsNearPosition(LevelPosition levelPosition)
    {
        var shiftX = levelPosition.X >> _chunkSizeBitShift;
        var shiftY = levelPosition.Y >> _chunkSizeBitShift;

        if (shiftX < 0 || shiftX >= _numberOfHorizontalChunks ||
            shiftY < 0 || shiftY >= _numberOfVerticalChunks)
            return new LargeSimpleSet<T>.Enumerator();

        var chunkPosition = new ChunkPosition(shiftX, shiftY);

        return _itemChunkLookup.TryGetValue(chunkPosition, out var itemChunk)
            ? itemChunk.GetEnumerator()
            : new LargeSimpleSet<T>.Enumerator();
    }

    public void UpdateItemPosition(int itemId, bool forceUpdate)
    {
        var item = _allItems[itemId];
        UpdateItemPosition(item, forceUpdate);
    }

    public void UpdateItemPosition(T item, bool forceUpdate)
    {
        var topLeftPixel = item.TopLeftPixel;
        var bottomRightPixel = item.BottomRightPixel;
        var previousTopLeftPixel = item.PreviousTopLeftPixel;
        var previousBottomRightPixel = item.PreviousBottomRightPixel;

        if (!forceUpdate &&
            topLeftPixel == previousTopLeftPixel &&
            bottomRightPixel == previousBottomRightPixel)
            return;

        GetShiftValues(topLeftPixel, out var topLeftShiftX, out var topLeftShiftY);
        GetShiftValues(bottomRightPixel, out var bottomRightShiftX, out var bottomRightShiftY);

        GetShiftValues(previousTopLeftPixel, out var previousTopLeftShiftX, out var previousTopLeftShiftY);
        GetShiftValues(previousBottomRightPixel, out var previousBottomRightShiftX, out var previousBottomRightShiftY);

        if (!forceUpdate &&
            topLeftShiftX == previousTopLeftShiftX &&
            topLeftShiftY == previousTopLeftShiftY &&
            bottomRightShiftX == previousBottomRightShiftX &&
            bottomRightShiftY == previousBottomRightShiftY)
            return;

        _chunksPositionScratchSpaceRemove.Clear();
        EvaluateChunkPositions(_chunksPositionScratchSpaceRemove, previousTopLeftShiftX, previousTopLeftShiftY, previousBottomRightShiftX, previousBottomRightShiftY);

        foreach (var itemChunkPosition in _chunksPositionScratchSpaceRemove.AsSpan())
        {
            if (!_itemChunkLookup.TryGetValue(itemChunkPosition, out var itemChunk))
                continue;

            itemChunk.Remove(item);
        }

        _chunkPositionScratchSpaceAdd.Clear();
        EvaluateChunkPositions(_chunkPositionScratchSpaceAdd, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);

        foreach (var itemChunkPosition in _chunkPositionScratchSpaceAdd.AsSpan())
        {
            if (!_itemChunkLookup.TryGetValue(itemChunkPosition, out var itemChunk))
            {
                itemChunk = new LargeSimpleSet<T>(_hasher);
                _itemChunkLookup.Add(itemChunkPosition, itemChunk);
            }

            itemChunk.Add(item);
        }
    }

    public void RemoveItem(T item)
    {
        var topLeftPixel = item.TopLeftPixel;
        var bottomRightPixel = item.BottomRightPixel;
        var previousTopLeftPixel = item.PreviousTopLeftPixel;
        var previousBottomRightPixel = item.PreviousBottomRightPixel;

        GetShiftValues(topLeftPixel, out var topLeftShiftX, out var topLeftShiftY);
        GetShiftValues(bottomRightPixel, out var bottomRightShiftX, out var bottomRightShiftY);

        GetShiftValues(previousTopLeftPixel, out var previousTopLeftShiftX, out var previousTopLeftShiftY);
        GetShiftValues(previousBottomRightPixel, out var previousBottomRightShiftX, out var previousBottomRightShiftY);

        _chunksPositionScratchSpaceRemove.Clear();
        EvaluateChunkPositions(_chunksPositionScratchSpaceRemove, previousTopLeftShiftX, previousTopLeftShiftY, previousBottomRightShiftX, previousBottomRightShiftY);

        foreach (var itemChunkPosition in _chunksPositionScratchSpaceRemove.AsSpan())
        {
            if (!_itemChunkLookup.TryGetValue(itemChunkPosition, out var itemChunk))
                continue;

            itemChunk.Remove(item);
        }

        if (topLeftShiftX == previousTopLeftShiftX &&
            topLeftShiftY == previousTopLeftShiftY &&
            bottomRightShiftX == previousBottomRightShiftX &&
            bottomRightShiftY == previousBottomRightShiftY)
            return;

        _chunkPositionScratchSpaceAdd.Clear();
        EvaluateChunkPositions(_chunkPositionScratchSpaceAdd, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);

        foreach (var itemChunkPosition in _chunkPositionScratchSpaceAdd.AsSpan())
        {
            if (!_itemChunkLookup.TryGetValue(itemChunkPosition, out var itemChunk))
                continue;

            itemChunk.Remove(item);
        }
    }

    public void PopulateSetWithItemsNearRegion(
        LargeSimpleSet<T> set,
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        GetShiftValues(topLeftLevelPosition, out var topLeftShiftX, out var topLeftShiftY);
        GetShiftValues(bottomRightLevelPosition, out var bottomRightShiftX, out var bottomRightShiftY);

        _setUnionChunkPositionUser.SetToUnionWith = set;
        EvaluateChunkPositions(_setUnionChunkPositionUser, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);
    }

    private void GetShiftValues(
        LevelPosition levelPosition,
        out int shiftX,
        out int shiftY)
    {
        shiftX = _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X) >> _chunkSizeBitShift;
        shiftY = _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y) >> _chunkSizeBitShift;
        shiftX = Math.Clamp(shiftX, 0, _numberOfHorizontalChunks - 1);
        shiftY = Math.Clamp(shiftY, 0, _numberOfVerticalChunks - 1);
    }

    private void EvaluateChunkPositions(IChunkPositionUser chunkPositionUser, int ax, int ay, int bx, int by)
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