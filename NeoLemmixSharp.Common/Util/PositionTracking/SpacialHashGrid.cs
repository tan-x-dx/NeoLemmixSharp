using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class SpacialHashGrid<TPerfectHasher, T>
    where TPerfectHasher : class, IPerfectHasher<T>
    where T : class, IRectangularBounds
{
    private readonly TPerfectHasher _hasher;
    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly SimpleSet<TPerfectHasher, T> _allTrackedItems;

    private readonly int _chunkSizeBitShift;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly int _bitArraySize;
    private readonly uint[] _cachedQueryScratchSpace;
    private readonly uint[] _allBits;

    private LevelPosition _cachedTopLeftChunkQuery;
    private LevelPosition _cachedBottomRightChunkQuery;
    private int _cachedQueryCount;

    public SpacialHashGrid(
        TPerfectHasher hasher,
        ChunkSizeType chunkSizeType,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _hasher = hasher;
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _allTrackedItems = new SimpleSet<TPerfectHasher, T>(hasher, false);

        _chunkSizeBitShift = chunkSizeType.ChunkSizeBitShiftFromType();
        var chunkSizeBitMask = (1 << _chunkSizeBitShift) - 1;

        _numberOfHorizontalChunks = (horizontalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >> _chunkSizeBitShift;
        _numberOfVerticalChunks = (verticalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >> _chunkSizeBitShift;

        _bitArraySize = BitArrayHelpers.CalculateBitArrayBufferLength(hasher.NumberOfItems);

        _cachedQueryScratchSpace = new uint[_bitArraySize];
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
    public SimpleSetEnumerable<TPerfectHasher, T> GetAllTrackedItems() => _allTrackedItems.AsSimpleEnumerable();

    public void Clear()
    {
        _allTrackedItems.Clear();
        new Span<uint>(_cachedQueryScratchSpace).Clear();
        new Span<uint>(_allBits).Clear();
        ClearCachedData();
    }

    public void GetAllItemsNearPosition(
        Span<uint> scratchSpaceSpan,
        LevelPosition levelPosition,
        out SimpleSetEnumerable<TPerfectHasher, T> result)
    {
        if (IsEmpty)
        {
            result = SimpleSetEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var chunkX = levelPosition.X >> _chunkSizeBitShift;
        var chunkY = levelPosition.Y >> _chunkSizeBitShift;

        if ((uint)chunkX >= (uint)_numberOfHorizontalChunks ||
            (uint)chunkY >= (uint)_numberOfVerticalChunks)
        {
            result = SimpleSetEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var sourceSpan = ReadOnlySpanForChunk(chunkX, chunkY);
        var queryCount = BitArrayHelpers.GetPopCount(sourceSpan);
        sourceSpan.CopyTo(scratchSpaceSpan);

        result = new SimpleSetEnumerable<TPerfectHasher, T>(_hasher, scratchSpaceSpan, queryCount);
    }

    /// <summary>
    /// Gets all items that overlap with the input region. Uses the span parameter to record data.
    /// </summary>
    /// <param name="scratchSpaceSpan">The span used to record data</param>
    /// <param name="levelRegion">The region to evaluate chunks from</param>
    /// <param name="result">The enumerable</param>
    /// <returns>An enumerable for items within the region</returns>
    public void GetAllItemsNearRegion(
        Span<uint> scratchSpaceSpan,
        LevelRegion levelRegion,
        out SimpleSetEnumerable<TPerfectHasher, T> result)
    {
        if (IsEmpty)
        {
            result = SimpleSetEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var topLeftChunk = GetChunkForPoint(levelRegion.GetTopLeftPosition());
        var bottomRightChunk = GetChunkForPoint(levelRegion.GetBottomRightPosition());

        if (topLeftChunk == _cachedTopLeftChunkQuery &&
            bottomRightChunk == _cachedBottomRightChunkQuery)
        {
            // If we've already got the data cached, just use it
            new ReadOnlySpan<uint>(_cachedQueryScratchSpace).CopyTo(scratchSpaceSpan);

            result = new SimpleSetEnumerable<TPerfectHasher, T>(_hasher, scratchSpaceSpan, _cachedQueryCount);
            return;
        }

        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work

            var sourceSpan = ReadOnlySpanForChunk(topLeftChunk.X, topLeftChunk.Y);
            sourceSpan.CopyTo(scratchSpaceSpan);
        }
        else
        {
            scratchSpaceSpan.Clear();
            EvaluateChunkUnions(scratchSpaceSpan, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
        }

        scratchSpaceSpan.CopyTo(_cachedQueryScratchSpace);
        _cachedTopLeftChunkQuery = topLeftChunk;
        _cachedBottomRightChunkQuery = bottomRightChunk;
        _cachedQueryCount = BitArrayHelpers.GetPopCount(scratchSpaceSpan);
        result = new SimpleSetEnumerable<TPerfectHasher, T>(_hasher, scratchSpaceSpan, _cachedQueryCount);
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Already tracking item!");

        var topLeftChunk = GetChunkForPoint(item.TopLeftPixel);
        var bottomRightChunk = GetChunkForPoint(item.BottomRightPixel);

        var p1 = new LevelRegion(_cachedTopLeftChunkQuery, _cachedBottomRightChunkQuery);
        var p2 = new LevelRegion(topLeftChunk, bottomRightChunk);
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

        var p1 = new LevelRegion(_cachedTopLeftChunkQuery, _cachedBottomRightChunkQuery);
        var p2 = new LevelRegion(topLeftChunk, bottomRightChunk);
        var p3 = new LevelRegion(previousTopLeftChunk, previousBottomRightChunk);
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

        ModifyChunks(ChunkOperationType.Add, item, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
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

        var p1 = new LevelRegion(_cachedTopLeftChunkQuery, _cachedBottomRightChunkQuery);
        var p2 = new LevelRegion(topLeftChunk, bottomRightChunk);
        var p3 = new LevelRegion(previousTopLeftChunk, previousBottomRightChunk);
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

        ModifyChunks(ChunkOperationType.Remove, item, topLeftChunk.X, topLeftChunk.Y, bottomRightChunk.X, bottomRightChunk.Y);
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
    private void ModifyChunks(ChunkOperationType chunkOperationType, T item, int ax, int ay, int bx, int by)
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
                ModifyChunkPosition(chunkOperationType, item, x1, y1);

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

    private void ModifyChunkPosition(ChunkOperationType chunkOperationType, T item, int x, int y)
    {
        var span = SpanForChunk(x, y);
        var hash = _hasher.Hash(item);
        if (chunkOperationType == ChunkOperationType.Add)
        {
            BitArrayHelpers.SetBit(span, hash);
        }
        else
        {
            BitArrayHelpers.ClearBit(span, hash);
        }
    }

    /// <summary>
    /// Performs a chunk union operation over a rectangle of chunks, placing the results into the span parameter.
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
    private void EvaluateChunkUnions(Span<uint> span, int ax, int ay, int bx, int by)
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

    private void ClearCachedData()
    {
        _cachedTopLeftChunkQuery = new LevelPosition(-256, -256);
        _cachedBottomRightChunkQuery = new LevelPosition(-256, -256);
    }

    private enum ChunkOperationType
    {
        Add,
        Remove
    }
}