using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.CodeAnalysis;
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

    private LevelPosition _topLeftChunkQuery;
    private LevelPosition _bottomRightChunkQuery;
    private int _chunkQueryCount;

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
        ClearCachedData();
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
        var chunk = new LevelPosition(chunkX, chunkY);
        if (chunk == _topLeftChunkQuery &&
            chunk == _bottomRightChunkQuery)
            return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, _chunkQueryCount);

        scratchSpaceSpan.Clear();

        var sourceSpan = ReadOnlySpanFor(chunkX, chunkY);
        _topLeftChunkQuery = chunk;
        _bottomRightChunkQuery = chunk;
        BitArray.UnionWith(scratchSpaceSpan, sourceSpan);
        _chunkQueryCount = BitArray.GetPopCount(scratchSpaceSpan);

        return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, _chunkQueryCount);
    }

    [Pure]
    public SimpleSetEnumerable<T> GetAllItemsNearRegion(LevelPositionPair levelRegion)
    {
        if (IsEmpty)
            return SimpleSetEnumerable<T>.Empty;

        var previousTopLeftChunkQuery = _topLeftChunkQuery;
        var previousBottomRightChunkQuery = _bottomRightChunkQuery;
        _topLeftChunkQuery = GetChunkForPoint(levelRegion.GetTopLeftPosition());
        _bottomRightChunkQuery = GetChunkForPoint(levelRegion.GetBottomRightPosition());

        var scratchSpaceSpan = new Span<uint>(_setUnionScratchSpace);

        if (previousTopLeftChunkQuery == _topLeftChunkQuery &&
            previousBottomRightChunkQuery == _bottomRightChunkQuery)
            return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, _chunkQueryCount);

        scratchSpaceSpan.Clear();

        if (_topLeftChunkQuery == _bottomRightChunkQuery)
        {
            // Only one chunk -> skip some extra work

            var sourceSpan = ReadOnlySpanFor(_topLeftChunkQuery.X, _bottomRightChunkQuery.Y);
            BitArray.UnionWith(scratchSpaceSpan, sourceSpan);
            _chunkQueryCount = BitArray.GetPopCount(scratchSpaceSpan);

            return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, _chunkQueryCount);
        }

        EvaluateChunkPositions(ChunkOperationType.Union, null, _topLeftChunkQuery.X, _topLeftChunkQuery.Y, _bottomRightChunkQuery.X, _bottomRightChunkQuery.Y);
        _chunkQueryCount = BitArray.GetPopCount(scratchSpaceSpan);

        return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, _chunkQueryCount);
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Item added twice!");

        var topLeftChunk = GetChunkForPoint(item.TopLeftPixel);
        var bottomRightChunk = GetChunkForPoint(item.BottomRightPixel);

        var p1 = new LevelPositionPair(_topLeftChunkQuery, _bottomRightChunkQuery);
        var p2 = new LevelPositionPair(topLeftChunk, bottomRightChunk);
        if (p1.Overlaps(p2))
        {
            ClearCachedData();
        }

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

        var p1 = new LevelPositionPair(_topLeftChunkQuery, _bottomRightChunkQuery);
        var p2 = new LevelPositionPair(topLeftChunk, bottomRightChunk);
        var p3 = new LevelPositionPair(previousTopLeftChunk, previousBottomRightChunk);
        if (p1.Overlaps(p2) ||
            p1.Overlaps(p3))
        {
            ClearCachedData();
        }

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

        EvaluateChunkPositions(ChunkOperationType.Add, item, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
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

        var p1 = new LevelPositionPair(_topLeftChunkQuery, _bottomRightChunkQuery);
        var p2 = new LevelPositionPair(topLeftChunk, bottomRightChunk);
        var p3 = new LevelPositionPair(previousTopLeftChunk, previousBottomRightChunk);
        if (p1.Overlaps(p2) ||
            p1.Overlaps(p3))
        {
            ClearCachedData();
        }

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

        EvaluateChunkPositions(ChunkOperationType.Remove, item, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
    }

    private LevelPosition GetChunkForPoint(LevelPosition levelPosition)
    {
        var chunkX = _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X) >> _chunkSizeBitShift;
        var chunkY = _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y) >> _chunkSizeBitShift;
        chunkX = Math.Clamp(chunkX, 0, _numberOfHorizontalChunks - 1);
        chunkY = Math.Clamp(chunkY, 0, _numberOfVerticalChunks - 1);

        return new LevelPosition(chunkX, chunkY);
    }

    /// <summary>
    /// Performs a chunk operation over a rectangle of chunks.
    ///
    /// <para>The rectangle of chunks begins with the top left coordinates of (<paramref name="ax" />, <paramref name="ay" />)
    /// down to the bottom right coordinates of (<paramref name="bx" />, <paramref name="by" />) inclusive.
    ///</para>
    /// 
    /// <para>If the b coordinates are less than their respective a coordinates, then this method wraps around and continues evaluating from zero.
    ///</para>
    /// </summary>
    /// <param name="chunkOperationType">The chunk operation to perform</param>
    /// <param name="item">An item to use in part of these chunk operations.
    /// Note: if performing the <see cref="ChunkOperationType.Add"/> or <see cref="ChunkOperationType.Remove"/> operations and this parameter is null,
    /// then an exception will be thrown.</param>
    /// <param name="ax">The top left x-coordinate.</param>
    /// <param name="ay">The top left y-coordinate.</param>
    /// <param name="bx">The bottom right x-coordinate.</param>
    /// <param name="by">The bottom right y-coordinate.</param>
    private void EvaluateChunkPositions(ChunkOperationType chunkOperationType, T? item, int ax, int ay, int bx, int by)
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

        while (x-- > 0)
        {
            var y1 = ay;
            var y = yCount;
            while (y-- > 0)
            {
                UseChunkPosition(chunkOperationType, item, x1, y1);

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

    private void UseChunkPosition(ChunkOperationType chunkOperationType, T? item, int x1, int y1)
    {
        switch (chunkOperationType)
        {
            case ChunkOperationType.Add:
                var addSpan = SpanFor(x1, y1);
                BitArray.SetBit(addSpan, _hasher.Hash(item!));
                return;

            case ChunkOperationType.Remove:
                var removeSpan = SpanFor(x1, y1);
                BitArray.ClearBit(removeSpan, _hasher.Hash(item!));
                return;

            case ChunkOperationType.Union:
                var readOnlySpan = ReadOnlySpanFor(x1, y1);
                BitArray.UnionWith(_setUnionScratchSpace, readOnlySpan);
                return;

            default:
                ThrowUnknownChunkPositionException(chunkOperationType);
                return;
        }
    }

    [DoesNotReturn]
    private static void ThrowUnknownChunkPositionException(ChunkOperationType chunkOperationType)
    {
        throw new ArgumentOutOfRangeException(nameof(chunkOperationType), chunkOperationType, "Unknown chunk position type");
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(int x, int y)
    {
        var index = _numberOfHorizontalChunks * y + x;
        index *= _bitArraySize;
        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearCachedData()
    {
        _topLeftChunkQuery = new LevelPosition(-256, -256);
        _bottomRightChunkQuery = new LevelPosition(-256, -256);
    }

    private enum ChunkOperationType
    {
        Add,
        Remove,
        Union
    }
}