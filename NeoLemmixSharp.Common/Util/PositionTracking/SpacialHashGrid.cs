using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class SpacialHashGrid<T> : IItemCountListener
    where T : class, IRectangularBounds
{
    private readonly IPerfectHasher<T> _hasher;
    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly SimpleSet<T> _allTrackedItems;

    private readonly int _chunkSizeBitShift;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private int _bitArraySize;
    private uint[] _setUnionScratchSpace;
    private uint[] _allBits;

    private LevelPosition _previousQueryTopLeftChunk;
    private LevelPosition _previousQueryBottomRightChunk;
    private int _previousQueryCount;

    public SpacialHashGrid(
        IPerfectHasher<T> hasher,
        ChunkSizeType chunkSizeType,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _hasher = hasher;
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _allTrackedItems = new SimpleSet<T>(hasher, false);

        _chunkSizeBitShift = chunkSizeType.ChunkSizeBitShiftFromType();
        var chunkSizeBitMask = (1 << _chunkSizeBitShift) - 1;

        _numberOfHorizontalChunks = (horizontalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >> _chunkSizeBitShift;
        _numberOfVerticalChunks = (verticalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >> _chunkSizeBitShift;

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
    public int ScratchSpaceSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _bitArraySize;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsTrackingItem(T item) => _allTrackedItems.Contains(item);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleSetEnumerable<T> GetAllTrackedItems() => _allTrackedItems.AsSimpleEnumerable();

    public void Clear()
    {
        _allTrackedItems.Clear();
        new Span<uint>(_setUnionScratchSpace).Clear();
        new Span<uint>(_allBits).Clear();
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

        var readonlyScratchSpaceSpan = new ReadOnlySpan<uint>(_setUnionScratchSpace);
        var chunk = new LevelPosition(chunkX, chunkY);
        if (chunk == _previousQueryTopLeftChunk &&
            chunk == _previousQueryBottomRightChunk)
            return new SimpleSetEnumerable<T>(_hasher, readonlyScratchSpaceSpan, _previousQueryCount);

        var sourceSpan = ReadOnlySpanForChunk(chunkX, chunkY);
        _previousQueryTopLeftChunk = chunk;
        _previousQueryBottomRightChunk = chunk;
        sourceSpan.CopyTo(new Span<uint>(_setUnionScratchSpace));
        _previousQueryCount = BitArrayHelpers.GetPopCount(sourceSpan);

        return new SimpleSetEnumerable<T>(_hasher, readonlyScratchSpaceSpan, _previousQueryCount);
    }

    /// <summary>
    /// Gets all items that overlap with the input region. Uses the backing scratch space to record data.
    /// </summary>
    /// <param name="levelRegion">The region to evaluate chunks from</param>
    /// <returns>An enumerable for items within the region</returns>
    [Pure]
    public SimpleSetEnumerable<T> GetAllItemsNearRegion(LevelPositionPair levelRegion)
    {
        if (IsEmpty)
            return SimpleSetEnumerable<T>.Empty;

        var previousTopLeftChunkQuery = _previousQueryTopLeftChunk;
        var previousBottomRightChunkQuery = _previousQueryBottomRightChunk;
        _previousQueryTopLeftChunk = GetChunkForPoint(levelRegion.GetTopLeftPosition());
        _previousQueryBottomRightChunk = GetChunkForPoint(levelRegion.GetBottomRightPosition());

        var readonlyScratchSpaceSpan = new ReadOnlySpan<uint>(_setUnionScratchSpace);
        if (previousTopLeftChunkQuery == _previousQueryTopLeftChunk &&
            previousBottomRightChunkQuery == _previousQueryBottomRightChunk)
            return new SimpleSetEnumerable<T>(_hasher, readonlyScratchSpaceSpan, _previousQueryCount);

        var scratchSpaceSpan = new Span<uint>(_setUnionScratchSpace);
        if (_previousQueryTopLeftChunk == _previousQueryBottomRightChunk)
        {
            // Only one chunk -> skip some extra work

            var sourceSpan = ReadOnlySpanForChunk(_previousQueryTopLeftChunk.X, _previousQueryBottomRightChunk.Y);
            sourceSpan.CopyTo(scratchSpaceSpan);
            _previousQueryCount = BitArrayHelpers.GetPopCount(sourceSpan);
        }
        else
        {
            scratchSpaceSpan.Clear();
            EvaluateChunks(ChunkOperationType.Union, null, _previousQueryTopLeftChunk.X, _previousQueryTopLeftChunk.Y, _previousQueryBottomRightChunk.X, _previousQueryBottomRightChunk.Y);
            _previousQueryCount = BitArrayHelpers.GetPopCount(readonlyScratchSpaceSpan);
        }

        return new SimpleSetEnumerable<T>(_hasher, readonlyScratchSpaceSpan, _previousQueryCount);
    }

    /// <summary>
    /// Gets all items that overlap with the input region. Uses the span parameter to record data.
    /// </summary>
    /// <param name="scratchSpaceSpan">The span used to record data</param>
    /// <param name="levelRegion">The region to evaluate chunks from</param>
    /// <returns>An enumerable for items within the region</returns>
    public SimpleSetEnumerable<T> GetAllItemsNearRegion(Span<uint> scratchSpaceSpan, LevelPositionPair levelRegion)
    {
        if (IsEmpty)
            return SimpleSetEnumerable<T>.Empty;

        var topLeftChunk = GetChunkForPoint(levelRegion.GetTopLeftPosition());
        var bottomRightChunk = GetChunkForPoint(levelRegion.GetBottomRightPosition());

        if (topLeftChunk == _previousQueryTopLeftChunk &&
            bottomRightChunk == _previousQueryBottomRightChunk)
        {
            // If we've already got the data cached, just use it
            new ReadOnlySpan<uint>(_setUnionScratchSpace).CopyTo(scratchSpaceSpan);

            return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, _previousQueryCount);
        }

        int queryCount;
        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work

            var sourceSpan = ReadOnlySpanForChunk(topLeftChunk.X, topLeftChunk.Y);
            sourceSpan.CopyTo(scratchSpaceSpan);
            queryCount = BitArrayHelpers.GetPopCount(sourceSpan);
        }
        else
        {
            scratchSpaceSpan.Clear();
            EvaluateChunks(scratchSpaceSpan, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);

            queryCount = BitArrayHelpers.GetPopCount(scratchSpaceSpan);
        }

        return new SimpleSetEnumerable<T>(_hasher, scratchSpaceSpan, queryCount);
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Already tracking item!");

        var topLeftChunk = GetChunkForPoint(item.TopLeftPixel);
        var bottomRightChunk = GetChunkForPoint(item.BottomRightPixel);

        var p1 = new LevelPositionPair(_previousQueryTopLeftChunk, _previousQueryBottomRightChunk);
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

        var p1 = new LevelPositionPair(_previousQueryTopLeftChunk, _previousQueryBottomRightChunk);
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

            var span = SpanForChunk(topLeftChunk.X, topLeftChunk.Y);
            BitArrayHelpers.SetBit(span, _hasher.Hash(item));

            return;
        }

        EvaluateChunks(ChunkOperationType.Add, item, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
    }

    public void RemoveItem(T item)
    {
        if (!_allTrackedItems.Remove(item))
            throw new InvalidOperationException("Not tracking item!");

        var topLeftChunk = GetChunkForPoint(item.TopLeftPixel);
        var bottomRightChunk = GetChunkForPoint(item.BottomRightPixel);

        DeregisterItemPosition(item, topLeftChunk, bottomRightChunk);

        var previousTopLeftChunk = GetChunkForPoint(item.PreviousTopLeftPixel);
        var previousBottomRightChunk = GetChunkForPoint(item.PreviousBottomRightPixel);

        var p1 = new LevelPositionPair(_previousQueryTopLeftChunk, _previousQueryBottomRightChunk);
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

            var span = SpanForChunk(topLeftChunk.X, topLeftChunk.Y);
            BitArrayHelpers.ClearBit(span, _hasher.Hash(item));

            return;
        }

        EvaluateChunks(ChunkOperationType.Remove, item, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
    }

    private LevelPosition GetChunkForPoint(LevelPosition levelPosition)
    {
        var chunkX = _horizontalBoundaryBehaviour.Normalise(levelPosition.X) >> _chunkSizeBitShift;
        var chunkY = _verticalBoundaryBehaviour.Normalise(levelPosition.Y) >> _chunkSizeBitShift;
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
    private void EvaluateChunks(ChunkOperationType chunkOperationType, T? item, int ax, int ay, int bx, int by)
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

    private void UseChunkPosition(ChunkOperationType chunkOperationType, T? item, int x, int y)
    {
        Span<uint> span;
        if (chunkOperationType == ChunkOperationType.Add)
        {
            span = SpanForChunk(x, y);
            BitArrayHelpers.SetBit(span, _hasher.Hash(item!));
            return;
        }

        if (chunkOperationType == ChunkOperationType.Remove)
        {
            span = SpanForChunk(x, y);
            BitArrayHelpers.ClearBit(span, _hasher.Hash(item!));
            return;
        }

        var readOnlySpan = ReadOnlySpanForChunk(x, y);
        span = new Span<uint>(_setUnionScratchSpace);
        BitArrayHelpers.UnionWith(span, readOnlySpan);
    }

    /// <summary>
    /// Performs a Union chunk operation over a rectangle of chunks, placing the results into the span parameter.
    ///
    /// <para>The rectangle of chunks begins with the top left coordinates of (<paramref name="ax" />, <paramref name="ay" />)
    /// down to the bottom right coordinates of (<paramref name="bx" />, <paramref name="by" />) inclusive.
    ///</para>
    /// 
    /// <para>If the b coordinates are less than their respective a coordinates, then this method wraps around and continues evaluating from zero.
    ///</para>
    /// </summary>
    /// 
    /// <param name="span">The span used to record data.</param>
    /// <param name="ax">The top left x-coordinate.</param>
    /// <param name="ay">The top left y-coordinate.</param>
    /// <param name="bx">The bottom right x-coordinate.</param>
    /// <param name="by">The bottom right y-coordinate.</param>
    private void EvaluateChunks(Span<uint> span, int ax, int ay, int bx, int by)
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
                BitArrayHelpers.UnionWith(span, ReadOnlySpanForChunk(x1, y1));

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

    [Pure]
    private Span<uint> SpanForChunk(int x, int y)
    {
        var index = IndexForChunk(x, y);
        return new Span<uint>(_allBits, index, _bitArraySize);
    }

    [Pure]
    private ReadOnlySpan<uint> ReadOnlySpanForChunk(int x, int y)
    {
        var index = IndexForChunk(x, y);
        return new ReadOnlySpan<uint>(_allBits, index, _bitArraySize);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexForChunk(int x, int y)
    {
        var index = _numberOfHorizontalChunks * y + x;
        index *= _bitArraySize;
        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearCachedData()
    {
        _previousQueryTopLeftChunk = new LevelPosition(-256, -256);
        _previousQueryBottomRightChunk = new LevelPosition(-256, -256);
    }

    private enum ChunkOperationType
    {
        Add,
        Remove,
        Union
    }

    public void OnNumberOfItemsChanged(int numberOfItems)
    {
        _allTrackedItems.OnNumberOfItemsChanged(numberOfItems);
        var newBitArraySize = _allTrackedItems.Size;

        if (_bitArraySize == newBitArraySize)
            return;

        var newBits = new uint[newBitArraySize * _numberOfHorizontalChunks * _numberOfVerticalChunks];

        TransferData(
            new ReadOnlySpan<uint>(_allBits),
            _bitArraySize,
            new Span<uint>(newBits),
            newBitArraySize);

        _bitArraySize = newBitArraySize;
        _allBits = newBits;
        Array.Resize(ref _setUnionScratchSpace, newBitArraySize);
    }

    private void TransferData(
        ReadOnlySpan<uint> oldData,
        int oldDataSize,
        Span<uint> newData,
        int newDataSize)
    {
        var limit = _numberOfHorizontalChunks * _numberOfVerticalChunks;

        for (var i = 0; i < limit; i++)
        {
            var oldChunk = oldData.Slice(i * oldDataSize, oldDataSize);
            var newChunk = newData.Slice(i * newDataSize, newDataSize);

            oldChunk.CopyTo(newChunk);
        }
    }
}