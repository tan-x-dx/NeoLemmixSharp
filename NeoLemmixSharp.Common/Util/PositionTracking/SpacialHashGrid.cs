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
    private delegate int UseChunkPosition(T item, int x, int y);

    private readonly IPerfectHasher<T> _hasher;
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly SimpleSet<T> _allTrackedItems;

    private readonly UseChunkPosition _doUnion;
    private readonly UseChunkPosition _doAdd;
    private readonly UseChunkPosition _doRemove;

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

        _doUnion = DoUnion;
        _doAdd = DoAdd;
        _doRemove = DoRemove;

        _chunkSizeBitShift = chunkSizeType.ChunkSizeBitShiftFromType();
        var chunkSizeBitMask = (1 << _chunkSizeBitShift) - 1;

        _numberOfHorizontalChunks = (horizontalBoundaryBehaviour.LevelWidth + chunkSizeBitMask) >> _chunkSizeBitShift;
        _numberOfVerticalChunks = (verticalBoundaryBehaviour.LevelHeight + chunkSizeBitMask) >> _chunkSizeBitShift;

        _bitArraySize = _allTrackedItems.Size;

        _setUnionScratchSpace = new uint[_bitArraySize];
        _allBits = new uint[_bitArraySize * _numberOfHorizontalChunks * _numberOfVerticalChunks];
    }

    [Pure]
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _allTrackedItems.Count == 0;
    }

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

    [Pure]
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

        return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, count);
    }

    [Pure]
    public SimpleSetEnumerable<T> GetAllItemsNearRegion(LevelPositionPair levelRegion)
    {
        if (IsEmpty)
            return SimpleSetEnumerable<T>.Empty;

        var topLeftChunk = GetChunkForPoint(levelRegion.GetTopLeftPosition());
        var bottomRightChunk = GetChunkForPoint(levelRegion.GetBottomRightPosition());

        var scratchSpaceSpan = new Span<uint>(_setUnionScratchSpace);
        scratchSpaceSpan.Clear();

        int count;
        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work

            var sourceSpan = ReadOnlySpanFor(topLeftChunk.X, topLeftChunk.Y);
            count = BitArray.UnionWith(scratchSpaceSpan, sourceSpan);

            return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, count);
        }

        count = EvaluateChunkPositions(_doUnion, null!, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
        return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, count);
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Item added twice!");

        var topLeftChunk = GetChunkForPoint(item.TopLeftPixel);
        var bottomRightChunk = GetChunkForPoint(item.BottomRightPixel);

        RegisterItemPosition(item, topLeftChunk, bottomRightChunk);
    }

    public void UpdateItemPosition(T item)
    {
        if (!_allTrackedItems.Contains(item))
            throw new InvalidOperationException("Item not registered!");

        var topLeftChunk = GetChunkForPoint(item.TopLeftPixel);
        var bottomRightChunk = GetChunkForPoint(item.BottomRightPixel);

        var previousTopLeftChunk = GetChunkForPoint(item.PreviousTopLeftPixel);
        var previousBottomRightChunk = GetChunkForPoint(item.PreviousBottomRightPixel);

        if (topLeftChunk == previousTopLeftChunk &&
            bottomRightChunk == previousBottomRightChunk)
            return;

        DeregisterItemPosition(item, previousTopLeftChunk, previousBottomRightChunk);
        RegisterItemPosition(item, topLeftChunk, bottomRightChunk);
    }

    private void RegisterItemPosition(T item, LevelPosition topLeftChunk, LevelPosition bottomRightChunk)
    {
        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work

            var span = SpanFor(topLeftChunk.X, topLeftChunk.Y);
            BitArray.SetBit(span, _hasher.Hash(item));

            return;
        }

        EvaluateChunkPositions(_doAdd, item, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
    }

    public void RemoveItem(T item)
    {
        if (!_allTrackedItems.Remove(item))
            throw new InvalidOperationException("Item not registered!");

        var topLeftChunk = GetChunkForPoint(item.TopLeftPixel);
        var bottomRightChunk = GetChunkForPoint(item.BottomRightPixel);

        DeregisterItemPosition(item, topLeftChunk, bottomRightChunk);

        var previousTopLeftChunk = GetChunkForPoint(item.PreviousTopLeftPixel);
        var previousBottomRightChunk = GetChunkForPoint(item.PreviousBottomRightPixel);

        if (topLeftChunk == previousTopLeftChunk &&
            bottomRightChunk == previousBottomRightChunk)
            return;

        DeregisterItemPosition(item, previousTopLeftChunk, previousBottomRightChunk);
    }

    private void DeregisterItemPosition(T item, LevelPosition topLeftChunk, LevelPosition bottomRightChunk)
    {
        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work

            var span = SpanFor(topLeftChunk.X, topLeftChunk.Y);
            BitArray.ClearBit(span, _hasher.Hash(item));

            return;
        }

        EvaluateChunkPositions(_doRemove, item, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
    }

    private LevelPosition GetChunkForPoint(LevelPosition levelPosition)
    {
        var chunkX = _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X) >> _chunkSizeBitShift;
        var chunkY = _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y) >> _chunkSizeBitShift;
        chunkX = Math.Clamp(chunkX, 0, _numberOfHorizontalChunks - 1);
        chunkY = Math.Clamp(chunkY, 0, _numberOfVerticalChunks - 1);

        return new LevelPosition(chunkX, chunkY);
    }

    private int EvaluateChunkPositions(UseChunkPosition useChunkPosition, T item, int ax, int ay, int bx, int by)
    {
        if (bx < ax)
        {
            bx += _numberOfHorizontalChunks;
        }

        if (by < ay)
        {
            by += _numberOfVerticalChunks;
        }

        var x = 1 + bx - ax;
        var x1 = ax;
        var yCount = 1 + by - ay;

        var result = 0;
        while (x-- > 0)
        {
            var y1 = ay;
            var y = yCount;
            while (y-- > 0)
            {
                result = useChunkPosition(item, x1, y1);

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

    private int DoUnion(T _, int x, int y)
    {
        var readOnlySpan = ReadOnlySpanFor(x, y);
        return BitArray.UnionWith(_setUnionScratchSpace, readOnlySpan);
    }

    private int DoAdd(T item, int x, int y)
    {
        var addSpan = SpanFor(x, y);
        BitArray.SetBit(addSpan, _hasher.Hash(item));
        return 0;
    }

    private int DoRemove(T item, int x, int y)
    {
        var removeSpan = SpanFor(x, y);
        BitArray.ClearBit(removeSpan, _hasher.Hash(item));
        return 0;
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
}