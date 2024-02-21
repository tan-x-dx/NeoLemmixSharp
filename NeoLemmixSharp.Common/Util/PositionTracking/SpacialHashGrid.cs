using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class SpacialHashGrid<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private readonly IPerfectHasher<T> _hasher;
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly SimpleSet<T> _allTrackedItems;

    private readonly int _chunkSizeBitShift;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;
    private readonly int _bitArraySize;

    private readonly uint[] _setUnionScratchSpace;
    private readonly uint[] _allBits;

    public SpacialHashGrid(
        IPerfectHasher<T> hasher,
        ChunkSizeType chunkSizeType,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _hasher = hasher;
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _allTrackedItems = new SimpleSet<T>(hasher);

        _chunkSizeBitShift = chunkSizeType.ChunkSizeBitShiftFromType();
        var chunkSizeBitMask = (1 << _chunkSizeBitShift) - 1;

        _numberOfHorizontalChunks = (horizontalBoundaryBehaviour.LevelWidth + chunkSizeBitMask) >> _chunkSizeBitShift;
        _numberOfVerticalChunks = (verticalBoundaryBehaviour.LevelHeight + chunkSizeBitMask) >> _chunkSizeBitShift;

        _bitArraySize = _allTrackedItems.Size;

        _setUnionScratchSpace = new uint[_bitArraySize];
        _allBits = new uint[_bitArraySize * _numberOfHorizontalChunks * _numberOfVerticalChunks];
    }

    [Pure]
    public bool IsEmpty => _allTrackedItems.Count == 0;

    [Pure]
    public bool IsTrackingItem(T item) => _allTrackedItems.Contains(item);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleSetEnumerable<T> GetAllTrackedItems() => _allTrackedItems.ToSimpleEnumerable();

    public void Clear()
    {
        _allTrackedItems.Clear();
        Array.Clear(_setUnionScratchSpace);
        Array.Clear(_allBits);
    }

    [Pure] // Technically not pure since it mutates the _setUnionScratchSpace array. But that's what it's intended for anyway so...
    public SimpleSetEnumerable<T> GetAllItemsNearPosition(LevelPosition levelPosition)
    {
        if (IsEmpty)
            return SimpleSetEnumerable<T>.Empty;

        var chunkX = levelPosition.X >> _chunkSizeBitShift;
        var chunkY = levelPosition.Y >> _chunkSizeBitShift;

        if (chunkX < 0 || chunkX >= _numberOfHorizontalChunks ||
            chunkY < 0 || chunkY >= _numberOfVerticalChunks)
            return SimpleSetEnumerable<T>.Empty;

        var scratchSpaceSpan = new Span<uint>(_setUnionScratchSpace);
        scratchSpaceSpan.Clear();

        var sourceSpan = ReadOnlySpanFor(chunkX, chunkY);
        var count = BitArray.UnionWith(scratchSpaceSpan, sourceSpan);

        return new SimpleSetEnumerable<T>(_hasher, new ReadOnlySpan<uint>(_setUnionScratchSpace), count);
    }

    [Pure]
    public SimpleSetEnumerable<T> GetAllItemsNearRegion(LevelPositionPair levelRegion)
    {
        if (IsEmpty)
            return SimpleSetEnumerable<T>.Empty;

        var topLeftLevelPosition = levelRegion.GetTopLeftPosition();
        var bottomRightLevelPosition = levelRegion.GetBottomRightPosition();

        GetShiftValues(topLeftLevelPosition, out var topLeftChunkX, out var topLeftChunkY);
        GetShiftValues(bottomRightLevelPosition, out var bottomRightChunkX, out var bottomRightChunkY);

        var scratchSpaceSpan = new Span<uint>(_setUnionScratchSpace);
        scratchSpaceSpan.Clear();

        // Only one chunk -> skip some extra work

        int count;
        if (topLeftChunkX == bottomRightChunkX &&
            topLeftChunkY == bottomRightChunkY)
        {
            var sourceSpan = ReadOnlySpanFor(topLeftChunkX, topLeftChunkY);
            count = BitArray.UnionWith(scratchSpaceSpan, sourceSpan);

            return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, count);
        }

        count = EvaluateChunkPositions(UseChunkPositionMode.Union, null, topLeftChunkX, topLeftChunkY, bottomRightChunkX, bottomRightChunkY);
        return new SimpleSetEnumerable<T>(_hasher, new ReadOnlySpan<uint>(_setUnionScratchSpace), count);
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

            var span = SpanFor(ax, ay);
            BitArray.SetBit(span, _hasher.Hash(item));

            return;
        }

        EvaluateChunkPositions(UseChunkPositionMode.Add, item, ax, ay, bx, by);
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

            var span = SpanFor(ax, ay);
            BitArray.ClearBit(span, _hasher.Hash(item));

            return;
        }

        EvaluateChunkPositions(UseChunkPositionMode.Remove, item, ax, ay, bx, by);
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

    private int EvaluateChunkPositions(UseChunkPositionMode useChunkPositionMode, T? item, int ax, int ay, int bx, int by)
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

        var result = 0;
        while (x-- > 0)
        {
            var y1 = ay;
            var y = yCount;
            while (y-- > 0)
            {
                result = UseChunkPosition(useChunkPositionMode, item, x1, y1);

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

        return result;
    }

    private int UseChunkPosition(UseChunkPositionMode useChunkPositionMode, T? item, int x, int y)
    {
        switch (useChunkPositionMode)
        {
            case UseChunkPositionMode.Add:
                var addSpan = SpanFor(x, y);
                BitArray.SetBit(addSpan, _hasher.Hash(item!));
                return 0;

            case UseChunkPositionMode.Remove:
                var removeSpan = SpanFor(x, y);
                BitArray.ClearBit(removeSpan, _hasher.Hash(item!));
                return 0;

            case UseChunkPositionMode.Union:
                var readOnlySpan = ReadOnlySpanFor(x, y);
                return BitArray.UnionWith(_setUnionScratchSpace, readOnlySpan);

            default:
                throw new ArgumentOutOfRangeException(nameof(useChunkPositionMode), useChunkPositionMode, "Invalid value");
        }
    }

    [Pure]
    private Span<uint> SpanFor(int x, int y)
    {
        var index = IndexFor(x, y);
        return new Span<uint>(_allBits, index, _bitArraySize);
    }

    [Pure]
    private ReadOnlySpan<uint> ReadOnlySpanFor(int x, int y)
    {
        var index = IndexFor(x, y);
        return new ReadOnlySpan<uint>(_allBits, index, _bitArraySize);
    }

    [Pure]
    private int IndexFor(int x, int y)
    {
        var index = _numberOfHorizontalChunks * y + x;
        index *= _bitArraySize;
        return index;
    }

    private enum UseChunkPositionMode
    {
        Add,
        Remove,
        Union
    }
}