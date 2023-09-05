using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util;

public sealed class PositionHelper<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private const int ChunkSizeBitShift = 6;
    private const int ChunkSize = 1 << ChunkSizeBitShift;
    private const int ChunkSizeBitMask = ChunkSize - 1;

    private readonly T[] _allItems;
    private readonly ISimpleHasher<T> _hasher;
    private readonly LargeSimpleSet<T>?[] _itemChunks;
    private readonly LargeBitArray _indicesOfItemChunks;

    private readonly LargeBitArray _indicesOfItemChunksScratchSpaceAdd;
    private readonly LargeBitArray _indicesOfItemChunksScratchSpaceRemove;

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly SetUnionChunkIndexUser _setUnionChunkIndexUser;
    private readonly ScratchSpaceChunkIndexUser _scratchSpaceAddChunkIndexUser;
    private readonly ScratchSpaceChunkIndexUser _scratchSpaceRemoveChunkIndexUser;

    public PositionHelper(
        T[] allItems,
        ISimpleHasher<T> hasher,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allItems = allItems;
        _hasher = hasher;

        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _numberOfHorizontalChunks = (_horizontalBoundaryBehaviour.LevelWidth + ChunkSizeBitMask) >> ChunkSizeBitShift;
        _numberOfVerticalChunks = (_verticalBoundaryBehaviour.LevelHeight + ChunkSizeBitMask) >> ChunkSizeBitShift;

        _itemChunks = new LargeSimpleSet<T>[_numberOfHorizontalChunks * _numberOfVerticalChunks];
        _indicesOfItemChunks = new LargeBitArray(_itemChunks.Length);
        _indicesOfItemChunksScratchSpaceAdd = new LargeBitArray(_itemChunks.Length);
        _indicesOfItemChunksScratchSpaceRemove = new LargeBitArray(_itemChunks.Length);

        _setUnionChunkIndexUser = new SetUnionChunkIndexUser(this);
        _scratchSpaceAddChunkIndexUser = new ScratchSpaceChunkIndexUser(_indicesOfItemChunksScratchSpaceAdd);
        _scratchSpaceRemoveChunkIndexUser = new ScratchSpaceChunkIndexUser(_indicesOfItemChunksScratchSpaceRemove);
    }

    [Pure]
    public LargeSimpleSet<T>.Enumerator GetAllItemIdsForPosition(LevelPosition levelPosition)
    {
        var shiftX = levelPosition.X >> ChunkSizeBitShift;
        var shiftY = levelPosition.Y >> ChunkSizeBitShift;

        if (shiftX < 0 || shiftX >= _numberOfHorizontalChunks ||
            shiftY < 0 || shiftY >= _numberOfVerticalChunks)
            return new LargeSimpleSet<T>.Enumerator();

        var index = _numberOfHorizontalChunks * shiftY + shiftX;
        var itemChunk = _itemChunks[index];
        return itemChunk?.GetEnumerator() ?? new LargeSimpleSet<T>.Enumerator();
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

        _indicesOfItemChunksScratchSpaceRemove.Clear();
        if (!forceUpdate)
        {
            GetShiftValues(previousTopLeftPixel, out var previousTopLeftShiftX, out var previousTopLeftShiftY);
            GetShiftValues(previousBottomRightPixel, out var previousBottomRightShiftX, out var previousBottomRightShiftY);

            if (topLeftShiftX == previousTopLeftShiftX &&
                topLeftShiftY == previousTopLeftShiftY &&
                bottomRightShiftX == previousBottomRightShiftX &&
                bottomRightShiftY == previousBottomRightShiftY)
                return;

            UseIndicesForIntervals(_scratchSpaceRemoveChunkIndexUser, previousTopLeftShiftX, previousTopLeftShiftY, previousBottomRightShiftX, previousBottomRightShiftY);
        }

        _indicesOfItemChunksScratchSpaceAdd.Clear();
        UseIndicesForIntervals(_scratchSpaceAddChunkIndexUser, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);

        var chunkIndexEnumerator = _indicesOfItemChunksScratchSpaceRemove.GetEnumerator();
        while (chunkIndexEnumerator.MoveNext())
        {
            var itemChunkIndex = chunkIndexEnumerator.Current;
            if (!_indicesOfItemChunksScratchSpaceAdd.GetBit(itemChunkIndex))
            {
                var itemChunk = _itemChunks[itemChunkIndex]!;

                itemChunk.Remove(item);
            }
        }

        chunkIndexEnumerator = _indicesOfItemChunksScratchSpaceAdd.GetEnumerator();
        while (chunkIndexEnumerator.MoveNext())
        {
            var itemChunkIndex = chunkIndexEnumerator.Current;
            if (!_indicesOfItemChunksScratchSpaceRemove.GetBit(itemChunkIndex))
            {
                ref var itemChunk = ref _itemChunks[itemChunkIndex];
                if (itemChunk == null)
                {
                    itemChunk = new LargeSimpleSet<T>(_hasher);
                    _indicesOfItemChunks.SetBit(itemChunkIndex);
                }

                itemChunk.Add(item);
            }
        }
    }

    public void PopulateSetWithItemsFromRegion(
        LargeSimpleSet<T> set,
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        GetShiftValues(topLeftLevelPosition, out var topLeftShiftX, out var topLeftShiftY);
        GetShiftValues(bottomRightLevelPosition, out var bottomRightShiftX, out var bottomRightShiftY);

        _setUnionChunkIndexUser.SetToUnionWith = set;
        UseIndicesForIntervals(_setUnionChunkIndexUser, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);
    }

    private void GetShiftValues(
        LevelPosition levelPosition,
        out int shiftX,
        out int shiftY)
    {
        shiftX = _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X) >> ChunkSizeBitShift;
        shiftY = _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y) >> ChunkSizeBitShift;
        shiftX = Math.Clamp(shiftX, 0, _numberOfHorizontalChunks - 1);
        shiftY = Math.Clamp(shiftY, 0, _numberOfVerticalChunks - 1);
    }

    private void UseIndicesForIntervals(IChunkIndexUser chunkIndexUser, int ax, int ay, int bx, int by)
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
                var index = _numberOfHorizontalChunks * y1 + x1;

                chunkIndexUser.UseChunkIndex(index);

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

    #region HelperTypes

    private interface IChunkIndexUser
    {
        void UseChunkIndex(int index);
    }

    private sealed class SetUnionChunkIndexUser : IChunkIndexUser
    {
        private readonly LargeSimpleSet<T>?[] _itemChunks;

        public LargeSimpleSet<T> SetToUnionWith { private get; set; }

        public SetUnionChunkIndexUser(PositionHelper<T> manager)
        {
            _itemChunks = manager._itemChunks;
        }

        public void UseChunkIndex(int index)
        {
            var itemSet = _itemChunks[index];
            if (itemSet is not null)
            {
                SetToUnionWith.UnionWith(itemSet);
            }
        }
    }

    private sealed class ScratchSpaceChunkIndexUser : IChunkIndexUser
    {
        private readonly LargeBitArray _indicesOfItemChunksScratchSpace;

        public ScratchSpaceChunkIndexUser(LargeBitArray indicesOfItemChunksScratchSpace)
        {
            _indicesOfItemChunksScratchSpace = indicesOfItemChunksScratchSpace;
        }

        public void UseChunkIndex(int index)
        {
            _indicesOfItemChunksScratchSpace.SetBit(index);
        }
    }

    #endregion
}