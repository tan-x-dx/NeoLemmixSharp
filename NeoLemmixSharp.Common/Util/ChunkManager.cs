using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util;

public sealed class ChunkManager<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private const int ChunkSizeBitShift = 6;
    private const int ChunkSize = 1 << ChunkSizeBitShift;
    private const int ChunkSizeBitMask = ChunkSize - 1;

    private readonly T[] _allItems;
    private readonly ISimpleHasher<T> _hasher;
    private readonly LargeSimpleSet<T>?[] _itemChunks;
    private readonly LargeBitArray _indicesOfItemChunks;

    private readonly LargeBitArray _indicesOfItemChunksScratchSpace;

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly SetUnionChunkIndexUser _setUnionChunkIndexUser;
    private readonly ScratchSpaceChunkIndexUser _scratchSpaceChunkIndexUser;

    public ChunkManager(
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
        _indicesOfItemChunksScratchSpace = new LargeBitArray(_itemChunks.Length);

        _setUnionChunkIndexUser = new SetUnionChunkIndexUser(this);
        _scratchSpaceChunkIndexUser = new ScratchSpaceChunkIndexUser(this);
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
        if (!forceUpdate &&
            item.TopLeftPixel == item.PreviousTopLeftPixel &&
            item.BottomRightPixel == item.PreviousBottomRightPixel)
            return;

        GetShiftValues(item.TopLeftPixel, out var topLeftShiftX, out var topLeftShiftY);
        GetShiftValues(item.BottomRightPixel, out var bottomRightShiftX, out var bottomRightShiftY);

        if (!forceUpdate)
        {
            GetShiftValues(item.PreviousTopLeftPixel, out var previousTopLeftShiftX, out var previousTopLeftShiftY);
            GetShiftValues(item.PreviousBottomRightPixel, out var previousBottomRightShiftX, out var previousBottomRightShiftY);

            if (topLeftShiftX == previousTopLeftShiftX &&
                topLeftShiftY == previousTopLeftShiftY &&
                bottomRightShiftX == previousBottomRightShiftX &&
                bottomRightShiftY == previousBottomRightShiftY)
                return;
        }

        _indicesOfItemChunksScratchSpace.Clear();
        UseIndicesForIntervals(_scratchSpaceChunkIndexUser, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);

        foreach (var itemChunkIndex in _indicesOfItemChunks)
        {
            var itemChunk = _itemChunks[itemChunkIndex]!;
            itemChunk.Remove(item);
        }

        foreach (var itemChunkIndex in _indicesOfItemChunksScratchSpace)
        {
            ref var itemChunk = ref _itemChunks[itemChunkIndex];
            itemChunk ??= new LargeSimpleSet<T>(_hasher);
            _indicesOfItemChunks.SetBit(itemChunkIndex);

            itemChunk.Add(item);
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
        // Is there a more elegant way to deal with all of these cases?

        if (ax <= bx &&
            ay <= by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = ay; y <= by; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }
            }
        }
        else if (ax > bx && ay <= by)
        {
            for (var y = ay; y <= by; y++)
            {
                for (var x = 0; x <= bx; x++)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }

                for (var x = _numberOfHorizontalChunks - 1; x >= ax; x--)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }
            }
        }
        else if (ax <= bx && ay > by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = 0; y <= by; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= ay; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }
            }
        }
        else
        {
            for (var x = 0; x <= ax; x++)
            {
                for (var y = 0; y <= by; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= ay; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }
            }

            for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
            {
                for (var y = 0; y <= by; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= ay; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;

                    chunkIndexUser.UseChunkIndex(index);
                }
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

        public SetUnionChunkIndexUser(ChunkManager<T> manager)
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
        private readonly LargeSimpleSet<T>?[] _itemChunks;
        private readonly LargeBitArray _indicesOfItemChunksScratchSpace;

        public ScratchSpaceChunkIndexUser(ChunkManager<T> manager)
        {
            _itemChunks = manager._itemChunks;
            _indicesOfItemChunksScratchSpace = manager._indicesOfItemChunksScratchSpace;
        }

        public void UseChunkIndex(int index)
        {
            var itemSet = _itemChunks[index];
            if (itemSet is not null)
            {
                _indicesOfItemChunksScratchSpace.SetBit(index);
            }
        }
    }

    #endregion
}