using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class PositionHelper<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private readonly int _chunkSizeBitShift;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly ISimpleHasher<T> _hasher;
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

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

        _itemChunkLookup = new Dictionary<ChunkPosition, LargeSimpleSet<T>>(ChunkPositionEqualityComparer.Instance);
        _chunkPositionScratchSpace = new SimpleChunkPositionList();
        _setUnionChunkPositionUser = new SetUnionChunkPositionUser<T>(_itemChunkLookup);
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

    public void AddItem(T item)
    {
        var topLeftPixel = item.TopLeftPixel;
        var bottomRightPixel = item.BottomRightPixel;

        GetShiftValues(topLeftPixel, out var topLeftShiftX, out var topLeftShiftY);
        GetShiftValues(bottomRightPixel, out var bottomRightShiftX, out var bottomRightShiftY);

        AddItem(item, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);
    }

    public void UpdateItemPosition(T item)
    {
        var topLeftPixel = item.TopLeftPixel;
        var bottomRightPixel = item.BottomRightPixel;
        var previousTopLeftPixel = item.PreviousTopLeftPixel;
        var previousBottomRightPixel = item.PreviousBottomRightPixel;

        if (topLeftPixel == previousTopLeftPixel &&
            bottomRightPixel == previousBottomRightPixel)
            return;

        GetShiftValues(topLeftPixel, out var topLeftShiftX, out var topLeftShiftY);
        GetShiftValues(bottomRightPixel, out var bottomRightShiftX, out var bottomRightShiftY);

        GetShiftValues(previousTopLeftPixel, out var previousTopLeftShiftX, out var previousTopLeftShiftY);
        GetShiftValues(previousBottomRightPixel, out var previousBottomRightShiftX, out var previousBottomRightShiftY);

        if (topLeftShiftX == previousTopLeftShiftX &&
            topLeftShiftY == previousTopLeftShiftY &&
            bottomRightShiftX == previousBottomRightShiftX &&
            bottomRightShiftY == previousBottomRightShiftY)
            return;

        RemoveItem(item, previousTopLeftShiftX, previousTopLeftShiftY, previousBottomRightShiftX, previousBottomRightShiftY);
        AddItem(item, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);
    }

    public void RemoveItem(T item)
    {
        var topLeftPixel = item.TopLeftPixel;
        var bottomRightPixel = item.BottomRightPixel;
        var previousTopLeftPixel = item.PreviousTopLeftPixel;
        var previousBottomRightPixel = item.PreviousBottomRightPixel;

        GetShiftValues(topLeftPixel, out var topLeftShiftX, out var topLeftShiftY);
        GetShiftValues(bottomRightPixel, out var bottomRightShiftX, out var bottomRightShiftY);

        RemoveItem(item, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);

        GetShiftValues(previousTopLeftPixel, out var previousTopLeftShiftX, out var previousTopLeftShiftY);
        GetShiftValues(previousBottomRightPixel, out var previousBottomRightShiftX, out var previousBottomRightShiftY);

        if (topLeftShiftX == previousTopLeftShiftX &&
            topLeftShiftY == previousTopLeftShiftY &&
            bottomRightShiftX == previousBottomRightShiftX &&
            bottomRightShiftY == previousBottomRightShiftY)
            return;

        RemoveItem(item, previousTopLeftShiftX, previousTopLeftShiftY, previousBottomRightShiftX, previousBottomRightShiftY);
    }

    private void AddItem(T item, int ax, int ay, int bx, int by)
    {
        _chunkPositionScratchSpace.Clear();
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

    private void RemoveItem(T item, int ax, int ay, int bx, int by)
    {
        _chunkPositionScratchSpace.Clear();
        EvaluateChunkPositions(_chunkPositionScratchSpace, ax, ay, bx, by);

        foreach (var itemChunkPosition in _chunkPositionScratchSpace.AsSpan())
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