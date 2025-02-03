using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Collections.BitBuffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class SpacialHashGrid<TPerfectHasher, T>
    where TPerfectHasher : class, IPerfectHasher<T>
    where T : class, IPreviousRectangularBounds
{
    private readonly TPerfectHasher _hasher;
    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly BitArraySet<TPerfectHasher, ArrayWrapper, T> _allTrackedItems;

    private readonly int _chunkSizeBitShift;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly int _bitArraySize;
    private readonly uint[] _cachedQueryScratchSpace;
    private readonly uint[] _allBits;

    private LevelPosition _cachedTopLeftChunkQuery = new(-256, -256);
    private LevelPosition _cachedBottomRightChunkQuery = new(-256, -256);
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

        _bitArraySize = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);

        var bitBuffer = new uint[_bitArraySize];
        _allTrackedItems = new BitArraySet<TPerfectHasher, ArrayWrapper, T>(_hasher, new ArrayWrapper(bitBuffer), false);

        _chunkSizeBitShift = chunkSizeType.ChunkSizeBitShiftFromType();
        var chunkSizeBitMask = (1 << _chunkSizeBitShift) - 1;

        _numberOfHorizontalChunks = (horizontalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >> _chunkSizeBitShift;
        _numberOfVerticalChunks = (verticalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >> _chunkSizeBitShift;

        _cachedQueryScratchSpace = new uint[_bitArraySize];
        _allBits = new uint[_bitArraySize * _numberOfHorizontalChunks * _numberOfVerticalChunks];
    }

    [Pure]
    public bool IsEmpty => _allTrackedItems.Count == 0;

    [Pure]
    public int ScratchSpaceSize => _bitArraySize;

    [Pure]
    public bool IsTrackingItem(T item) => _allTrackedItems.Contains(item);

    [Pure]
    public BitArrayEnumerable<TPerfectHasher, T> GetAllTrackedItems() => _allTrackedItems.AsEnumerable();

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
        out BitArrayEnumerable<TPerfectHasher, T> result)
    {
        if (IsEmpty)
        {
            result = BitArrayEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var chunkX = levelPosition.X >> _chunkSizeBitShift;
        var chunkY = levelPosition.Y >> _chunkSizeBitShift;

        if ((uint)chunkX >= (uint)_numberOfHorizontalChunks ||
            (uint)chunkY >= (uint)_numberOfVerticalChunks)
        {
            result = BitArrayEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var sourceSpan = ReadOnlySpanForChunk(chunkX, chunkY);
        var queryCount = BitArrayHelpers.GetPopCount(sourceSpan);
        sourceSpan.CopyTo(scratchSpaceSpan);

        result = new BitArrayEnumerable<TPerfectHasher, T>(_hasher, scratchSpaceSpan, queryCount);
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
        out BitArrayEnumerable<TPerfectHasher, T> result)
    {
        if (IsEmpty)
        {
            result = BitArrayEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var topLeftChunk = GetTopLeftChunkForRegion(levelRegion);
        var bottomRightChunk = GetBottomRightChunkForRegion(levelRegion);

        if (_cachedTopLeftChunkQuery == topLeftChunk &&
            _cachedBottomRightChunkQuery == bottomRightChunk)
        {
            // If we've already got the data cached, just use it
            new ReadOnlySpan<uint>(_cachedQueryScratchSpace).CopyTo(scratchSpaceSpan);

            result = new BitArrayEnumerable<TPerfectHasher, T>(_hasher, scratchSpaceSpan, _cachedQueryCount);
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
        result = new BitArrayEnumerable<TPerfectHasher, T>(_hasher, scratchSpaceSpan, _cachedQueryCount);
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Already tracking item!");

        var currentBounds = item.CurrentBounds;
        var topLeftChunk = GetTopLeftChunkForRegion(currentBounds);
        var bottomRightChunk = GetBottomRightChunkForRegion(currentBounds);

        ClearCachedData();

        RegisterItemPosition(item, topLeftChunk, bottomRightChunk);
    }

    public void UpdateItemPosition(T item)
    {
        if (!_allTrackedItems.Contains(item))
            throw new InvalidOperationException("Item not registered!");

        var currentBounds = item.CurrentBounds;
        var currentTopLeftChunk = GetTopLeftChunkForRegion(currentBounds);
        var currentBottomRightChunk = GetBottomRightChunkForRegion(currentBounds);

        var previousBounds = item.PreviousBounds;
        var previousTopLeftChunk = GetTopLeftChunkForRegion(previousBounds);
        var previousBottomRightChunk = GetBottomRightChunkForRegion(previousBounds);

        if (currentTopLeftChunk == previousTopLeftChunk &&
            currentBottomRightChunk == previousBottomRightChunk)
            return;

        ClearCachedData();

        DeregisterItemPosition(item, previousTopLeftChunk, previousBottomRightChunk);
        RegisterItemPosition(item, currentTopLeftChunk, currentBottomRightChunk);
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

        var currentBounds = item.CurrentBounds;
        var currentTopLeftChunk = GetTopLeftChunkForRegion(currentBounds);
        var currentBottomRightChunk = GetBottomRightChunkForRegion(currentBounds);

        DeregisterItemPosition(item, currentTopLeftChunk, currentBottomRightChunk);

        var previousBounds = item.PreviousBounds;
        var previousTopLeftChunk = GetTopLeftChunkForRegion(previousBounds);
        var previousBottomRightChunk = GetBottomRightChunkForRegion(previousBounds);

        ClearCachedData();

        if (currentTopLeftChunk == previousTopLeftChunk &&
            currentBottomRightChunk == previousBottomRightChunk)
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

    private LevelPosition GetTopLeftChunkForRegion(LevelRegion levelRegion)
    {
        return ConvertToChunkPosition(levelRegion.P);
    }

    private LevelPosition GetBottomRightChunkForRegion(LevelRegion levelRegion)
    {
        return ConvertToChunkPosition(levelRegion.GetBottomRight());
    }

    private LevelPosition ConvertToChunkPosition(LevelPosition position)
    {
        var x = _horizontalBoundaryBehaviour.Normalise(position.X) >> _chunkSizeBitShift;
        var y = _verticalBoundaryBehaviour.Normalise(position.Y) >> _chunkSizeBitShift;

        return new LevelPosition(x, y);
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

        var y = 1 + by - ay;
        var y1 = ay;
        var xCount = 1 + bx - ax;

        while (y-- > 0)
        {
            var x1 = ax;
            var x = xCount;
            while (x-- > 0)
            {
                ModifyChunkPosition(chunkOperationType, item, x1, y1);

                if (++x1 == _numberOfHorizontalChunks)
                {
                    x1 = 0;
                }
            }

            if (++y1 == _numberOfVerticalChunks)
            {
                y1 = 0;
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

        var y = 1 + by - ay;
        var y1 = ay;
        var xCount = 1 + bx - ax;

        while (y-- > 0)
        {
            var x1 = ax;
            var x = xCount;
            while (x-- > 0)
            {
                BitArrayHelpers.UnionWith(span, ReadOnlySpanForChunk(x1, y1));

                if (++x1 == _numberOfHorizontalChunks)
                {
                    x1 = 0;
                }
            }

            if (++y1 == _numberOfVerticalChunks)
            {
                y1 = 0;
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
        _cachedTopLeftChunkQuery = new LevelPosition(-256, -256);
        _cachedBottomRightChunkQuery = new LevelPosition(-256, -256);
    }

    private enum ChunkOperationType
    {
        Add,
        Remove
    }
}