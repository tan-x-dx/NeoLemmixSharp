using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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

        var topLeft = NormalisePosition(item.TopLeftPixel);
        GetShiftValues(topLeft, out var topLeftShiftX, out var topLeftShiftY);

        var bottomRight = NormalisePosition(item.BottomRightPixel);
        GetShiftValues(bottomRight, out var bottomRightShiftX, out var bottomRightShiftY);

        if (!forceUpdate)
        {
            var previousTopLeft = NormalisePosition(item.PreviousTopLeftPixel);
            GetShiftValues(previousTopLeft, out var previousTopLeftShiftX, out var previousTopLeftShiftY);

            var previousBottomRight = NormalisePosition(item.PreviousBottomRightPixel);
            GetShiftValues(previousBottomRight, out var previousBottomRightShiftX, out var previousBottomRightShiftY);

            if (topLeftShiftX == previousTopLeftShiftX &&
                topLeftShiftY == previousTopLeftShiftY &&
                bottomRightShiftX == previousBottomRightShiftX &&
                bottomRightShiftY == previousBottomRightShiftY)
                return;
        }

        WriteToItemChunkScratchSpace(topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);

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

    private void WriteToItemChunkScratchSpace(int ax, int ay, int bx, int by)
    {
        _indicesOfItemChunksScratchSpace.Clear();

        // Is there a more elegant way to deal with all of these cases?

        if (ax <= bx &&
            ay <= by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = ay; y <= by; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }
        }
        else if (ax > bx && ay <= by)
        {
            for (var y = ay; y <= by; y++)
            {
                for (var x = 0; x <= ax; x++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }

                for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }
        }
        else if (ax <= bx && ay > by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }
        }
        else
        {
            for (var x = 0; x <= ax; x++)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }

            for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }
        }
    }

    private void GetShiftValues(
        LevelPosition levelPosition,
        out int shiftX,
        out int shiftY)
    {
        var topLeft = NormalisePosition(levelPosition);

        shiftX = topLeft.X >> ChunkSizeBitShift;
        shiftY = topLeft.Y >> ChunkSizeBitShift;
        shiftX = Math.Clamp(shiftX, 0, _numberOfHorizontalChunks - 1);
        shiftY = Math.Clamp(shiftY, 0, _numberOfVerticalChunks - 1);
    }

    public void PopulateSetWithItemsFromRegion(
        LargeSimpleSet<T> set,
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        topLeftLevelPosition = NormalisePosition(topLeftLevelPosition);
        GetShiftValues(topLeftLevelPosition, out var topLeftShiftX, out var topLeftShiftY);

        bottomRightLevelPosition = NormalisePosition(bottomRightLevelPosition);
        GetShiftValues(bottomRightLevelPosition, out var bottomRightShiftX, out var bottomRightShiftY);

        WriteToSet(set, topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);
    }

    private void WriteToSet(LargeSimpleSet<T> set, int ax, int ay, int bx, int by)
    {
        _indicesOfItemChunksScratchSpace.Clear();

        // Is there a more elegant way to deal with all of these cases?

        if (ax <= bx &&
            ay <= by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = ay; y <= by; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }
            }
        }
        else if (ax > bx && ay <= by)
        {
            for (var y = ay; y <= by; y++)
            {
                for (var x = 0; x <= ax; x++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }

                for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }
            }
        }
        else if (ax <= bx && ay > by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }
            }
        }
        else
        {
            for (var x = 0; x <= ax; x++)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }
            }

            for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    var itemSet = _itemChunks[index];
                    if (itemSet is not null)
                    {
                        set.UnionWith(itemSet);
                    }
                }
            }
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        return new LevelPosition(
            _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X),
            _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y));
    }
}