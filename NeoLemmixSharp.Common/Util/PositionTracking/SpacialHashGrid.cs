using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public sealed class SpacialHashGrid<TPerfectHasher, T>
    where TPerfectHasher : class, IPerfectHasher<T>, IBitBufferCreator<ArrayBitBuffer>
    where T : class, IPreviousRectangularBounds
{
    private readonly TPerfectHasher _hasher;
    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly BitArraySet<TPerfectHasher, ArrayBitBuffer, T> _allTrackedItems;

    private readonly int _chunkSizeBitShift;
    private readonly LevelSize _sizeInChunks;

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
        _allTrackedItems = new BitArraySet<TPerfectHasher, ArrayBitBuffer, T>(_hasher, false);

        _chunkSizeBitShift = chunkSizeType.ChunkSizeBitShiftFromType();
        var chunkSizeBitMask = (1 << _chunkSizeBitShift) - 1;

        _sizeInChunks = new LevelSize(
            (horizontalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >>> _chunkSizeBitShift,
            (verticalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >>> _chunkSizeBitShift);

        _cachedQueryScratchSpace = new uint[_bitArraySize];
        _allBits = new uint[_bitArraySize * _sizeInChunks.Area()];
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

        var chunkPosition = new LevelPosition(levelPosition.X >> _chunkSizeBitShift, levelPosition.Y >> _chunkSizeBitShift);

        if (!_sizeInChunks.EncompassesPoint(chunkPosition))
        {
            result = BitArrayEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var sourceSpan = ReadOnlySpanForChunk(chunkPosition);
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

            var sourceSpan = ReadOnlySpanForChunk(topLeftChunk);
            sourceSpan.CopyTo(scratchSpaceSpan);
        }
        else
        {
            scratchSpaceSpan.Clear();
            EvaluateChunkUnions(scratchSpaceSpan, topLeftChunk, bottomRightChunk);
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

            var span = SpanForChunk(topLeftChunk);
            BitArrayHelpers.SetBit(span, _hasher.Hash(item));

            return;
        }

        ModifyChunks(ChunkOperationType.Add, item, topLeftChunk, bottomRightChunk);
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

            var span = SpanForChunk(topLeftChunk);
            BitArrayHelpers.ClearBit(span, _hasher.Hash(item));

            return;
        }

        ModifyChunks(ChunkOperationType.Remove, item, topLeftChunk, bottomRightChunk);
    }

    private LevelPosition GetTopLeftChunkForRegion(LevelRegion levelRegion)
    {
        return ConvertToChunkPosition(levelRegion.Position);
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
    /// <para>The rectangle of chunks begins with the top left coordinates of <paramref name="chunkA" />
    /// down to the bottom right coordinates of <paramref name="chunkB" /> inclusive.
    ///</para>
    /// 
    /// <para>If the <paramref name="chunkB" /> coordinates are less than their respective <paramref name="chunkA" /> coordinates, then this method wraps around and continues evaluating from zero.
    ///</para>
    /// </summary>
    /// <param name="chunkOperationType">The chunk operation to perform</param>
    /// <param name="item">An item to use in part of these chunk operations.
    /// Note: if performing the <see cref="ChunkOperationType.Add"/> or <see cref="ChunkOperationType.Remove"/> operations and this parameter is null,
    /// then an exception will be thrown.</param>
    /// <param name="chunkA">The top left position.</param>
    /// <param name="chunkB">The bottom right position.</param>
    private void ModifyChunks(ChunkOperationType chunkOperationType, T item, LevelPosition chunkA, LevelPosition chunkB)
    {
        if (chunkB.X < chunkA.X)
        {
            chunkB = new LevelPosition(chunkB.X + _sizeInChunks.W, chunkB.Y);
        }

        if (chunkB.Y < chunkA.Y)
        {
            chunkB = new LevelPosition(chunkB.X, chunkB.Y + _sizeInChunks.H);
        }

        var y = 1 + chunkB.Y - chunkA.Y;
        var y1 = chunkA.Y;
        var xCount = 1 + chunkB.X - chunkA.X;

        while (y-- > 0)
        {
            var x1 = chunkA.X;
            var x = xCount;
            while (x-- > 0)
            {
                ModifyChunkPosition(chunkOperationType, item, new LevelPosition(x1, y1));

                if (++x1 == _sizeInChunks.W)
                {
                    x1 = 0;
                }
            }

            if (++y1 == _sizeInChunks.H)
            {
                y1 = 0;
            }
        }
    }

    private void ModifyChunkPosition(ChunkOperationType chunkOperationType, T item, LevelPosition chunkPosition)
    {
        var span = SpanForChunk(chunkPosition);
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
    /// <para>The rectangle of chunks begins with the top left coordinates of <paramref name="chunkA" />
    /// down to the bottom right coordinates of <paramref name="chunkB" /> inclusive.
    ///</para>
    /// 
    /// <para>If the <paramref name="chunkB" /> coordinates are less than their respective <paramref name="chunkA" /> coordinates, then this method wraps around and continues evaluating from zero.
    ///</para>
    /// </summary>
    /// 
    /// <param name="span">The span used to record data.</param>
    /// <param name="chunkA">The top left position.</param>
    /// <param name="chunkB">The bottom right position.</param>
    private void EvaluateChunkUnions(Span<uint> span, LevelPosition chunkA, LevelPosition chunkB)
    {
        if (chunkB.X < chunkA.X)
        {
            chunkB = new LevelPosition(chunkB.X + _sizeInChunks.W, chunkB.Y);
        }

        if (chunkB.Y < chunkA.Y)
        {
            chunkB = new LevelPosition(chunkB.X, chunkB.Y + _sizeInChunks.H);
        }

        var y = 1 + chunkB.Y - chunkA.Y;
        var y1 = chunkA.Y;
        var xCount = 1 + chunkB.X - chunkA.X;

        while (y-- > 0)
        {
            var x1 = chunkA.X;
            var x = xCount;
            while (x-- > 0)
            {
                BitArrayHelpers.UnionWith(span, ReadOnlySpanForChunk(new LevelPosition(x1, y1)));

                if (++x1 == _sizeInChunks.W)
                {
                    x1 = 0;
                }
            }

            if (++y1 == _sizeInChunks.H)
            {
                y1 = 0;
            }
        }
    }

    [Pure]
    private Span<uint> SpanForChunk(LevelPosition pos)
    {
        var index = IndexForChunk(pos);
        return new Span<uint>(_allBits, index, _bitArraySize);
    }

    [Pure]
    private ReadOnlySpan<uint> ReadOnlySpanForChunk(LevelPosition pos)
    {
        var index = IndexForChunk(pos);
        return new ReadOnlySpan<uint>(_allBits, index, _bitArraySize);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexForChunk(LevelPosition pos)
    {
        var index = _sizeInChunks.GetIndexOfPoint(pos);
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