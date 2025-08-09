using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public unsafe sealed class SpacialHashGrid<TPerfectHasher, T> : IDisposable
    where TPerfectHasher : class, IPerfectHasher<T>, IBitBufferCreator<ArrayBitBuffer>
    where T : class, IRectangularBounds
{
    private readonly TPerfectHasher _hasher;
    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly BitArraySet<TPerfectHasher, ArrayBitBuffer, T> _allTrackedItems;

    private readonly int _chunkSizeBitShift;
    private readonly Size _sizeInChunks;

    private readonly int _bitArraySize;
    private readonly uint* _cachedQueryScratchSpacePointer;
    private readonly uint* _allBitsPointer;
    private readonly RectangularRegion* _previousItemPositionsPointer;

    private Point _cachedTopLeftChunkQuery = new(-256, -256);
    private Point _cachedBottomRightChunkQuery = new(-256, -256);
    private int _cachedQueryCount;

    public SpacialHashGrid(
        TPerfectHasher hasher,
        ChunkSize chunkSize,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _hasher = hasher;
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _bitArraySize = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        _allTrackedItems = new BitArraySet<TPerfectHasher, ArrayBitBuffer, T>(_hasher);

        _chunkSizeBitShift = chunkSize.GetChunkSizeBitShift();
        var chunkSizeBitMask = (1 << _chunkSizeBitShift) - 1;

        _sizeInChunks = new Size(
            (horizontalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >>> _chunkSizeBitShift,
            (verticalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >>> _chunkSizeBitShift);

        _cachedQueryScratchSpacePointer = (uint*)Marshal.AllocHGlobal(ScratchSpaceSize * sizeof(uint));
        _allBitsPointer = (uint*)Marshal.AllocHGlobal(AllBitsSize * sizeof(uint));
        _previousItemPositionsPointer = (RectangularRegion*)Marshal.AllocHGlobal(_hasher.NumberOfItems * sizeof(RectangularRegion));

        Clear();
    }

    [Pure]
    public bool IsEmpty => _allTrackedItems.Count == 0;

    [Pure]
    public int ScratchSpaceSize => _bitArraySize;

    [Pure]
    private int AllBitsSize => _bitArraySize * _sizeInChunks.Area();

    [Pure]
    public bool IsTrackingItem(T item) => _allTrackedItems.Contains(item);

    [Pure]
    public BitArrayEnumerable<TPerfectHasher, T> GetAllTrackedItems() => _allTrackedItems.AsEnumerable();

    public void Clear()
    {
        _allTrackedItems.Clear();
        new Span<uint>(_cachedQueryScratchSpacePointer, ScratchSpaceSize).Clear();
        new Span<uint>(_allBitsPointer, AllBitsSize).Clear();
        new Span<RectangularRegion>(_previousItemPositionsPointer, _hasher.NumberOfItems).Clear();
        ClearCachedData();
    }

    public void GetAllItemsNearPosition(
        Span<uint> scratchSpaceSpan,
        Point levelPosition,
        out BitArrayEnumerable<TPerfectHasher, T> result)
    {
        if (IsEmpty)
        {
            result = BitArrayEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var chunkPosition = new Point(levelPosition.X >> _chunkSizeBitShift, levelPosition.Y >> _chunkSizeBitShift);

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
        RectangularRegion levelRegion,
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
            new ReadOnlySpan<uint>(_cachedQueryScratchSpacePointer, ScratchSpaceSize).CopyTo(scratchSpaceSpan);

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

        scratchSpaceSpan.CopyTo(new Span<uint>(_cachedQueryScratchSpacePointer, ScratchSpaceSize));
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

        RegisterItemPosition(item, topLeftChunk, bottomRightChunk, ref GetPreviousBoundsForItem(item));
    }

    public void UpdateItemPosition(T item)
    {
        if (!_allTrackedItems.Contains(item))
            throw new InvalidOperationException("Item not registered!");

        var currentBounds = item.CurrentBounds;
        var currentTopLeftChunk = GetTopLeftChunkForRegion(currentBounds);
        var currentBottomRightChunk = GetBottomRightChunkForRegion(currentBounds);

        ref var previousBounds = ref GetPreviousBoundsForItem(item);
        var previousTopLeftChunk = GetTopLeftChunkForRegion(previousBounds);
        var previousBottomRightChunk = GetBottomRightChunkForRegion(previousBounds);

        if (currentTopLeftChunk == previousTopLeftChunk &&
            currentBottomRightChunk == previousBottomRightChunk)
            return;

        ClearCachedData();

        DeregisterItemPosition(item, previousTopLeftChunk, previousBottomRightChunk);
        RegisterItemPosition(item, currentTopLeftChunk, currentBottomRightChunk, ref previousBounds);
    }

    private void RegisterItemPosition(T item, Point topLeftChunk, Point bottomRightChunk, ref RectangularRegion previousBounds)
    {
        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work
            previousBounds = new RectangularRegion(topLeftChunk);
            var span = SpanForChunk(topLeftChunk);
            BitArrayHelpers.SetBit(span, _hasher.Hash(item));

            return;
        }

        previousBounds = new RectangularRegion(topLeftChunk, bottomRightChunk);
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

        ref var previousBounds = ref GetPreviousBoundsForItem(item);
        var previousTopLeftChunk = GetTopLeftChunkForRegion(previousBounds);
        var previousBottomRightChunk = GetBottomRightChunkForRegion(previousBounds);

        ClearCachedData();

        if (currentTopLeftChunk == previousTopLeftChunk &&
            currentBottomRightChunk == previousBottomRightChunk)
            return;

        DeregisterItemPosition(item, previousTopLeftChunk, previousBottomRightChunk);
        previousBounds = new RectangularRegion();
    }

    private void DeregisterItemPosition(T item, Point topLeftChunk, Point bottomRightChunk)
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

    private Point GetTopLeftChunkForRegion(RectangularRegion levelRegion)
    {
        return ConvertToChunkPosition(levelRegion.Position);
    }

    private Point GetBottomRightChunkForRegion(RectangularRegion levelRegion)
    {
        return ConvertToChunkPosition(levelRegion.GetBottomRight());
    }

    private Point ConvertToChunkPosition(Point position)
    {
        var x = _horizontalBoundaryBehaviour.Normalise(position.X) >> _chunkSizeBitShift;
        var y = _verticalBoundaryBehaviour.Normalise(position.Y) >> _chunkSizeBitShift;

        return new Point(x, y);
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
    /// <param name="chunkA">The top left position.</param>
    /// <param name="chunkB">The bottom right position.</param>
    private void ModifyChunks(ChunkOperationType chunkOperationType, T item, Point chunkA, Point chunkB)
    {
        if (chunkB.X < chunkA.X)
        {
            chunkB = new Point(chunkB.X + _sizeInChunks.W, chunkB.Y);
        }

        if (chunkB.Y < chunkA.Y)
        {
            chunkB = new Point(chunkB.X, chunkB.Y + _sizeInChunks.H);
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
                ModifyChunkPosition(chunkOperationType, item, new Point(x1, y1));

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

    private void ModifyChunkPosition(ChunkOperationType chunkOperationType, T item, Point chunkPosition)
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
    private void EvaluateChunkUnions(Span<uint> span, Point chunkA, Point chunkB)
    {
        if (chunkB.X < chunkA.X)
        {
            chunkB = new Point(chunkB.X + _sizeInChunks.W, chunkB.Y);
        }

        if (chunkB.Y < chunkA.Y)
        {
            chunkB = new Point(chunkB.X, chunkB.Y + _sizeInChunks.H);
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
                BitArrayHelpers.UnionWith(span, ReadOnlySpanForChunk(new Point(x1, y1)));

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

    private ref RectangularRegion GetPreviousBoundsForItem(T item)
    {
        var index = _hasher.Hash(item);
        RectangularRegion* pointer = _previousItemPositionsPointer;
        pointer += index;

        return ref *pointer;
    }

    [Pure]
    private Span<uint> SpanForChunk(Point pos)
    {
        var index = IndexForChunk(pos);
        uint* pointer = _allBitsPointer;
        pointer += index;

        return new Span<uint>(pointer, ScratchSpaceSize);
    }

    [Pure]
    private ReadOnlySpan<uint> ReadOnlySpanForChunk(Point pos)
    {
        var index = IndexForChunk(pos);
        uint* pointer = _allBitsPointer;
        pointer += index;

        return new ReadOnlySpan<uint>(pointer, ScratchSpaceSize);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexForChunk(Point pos)
    {
        var index = _sizeInChunks.GetIndexOfPoint(pos);
        index *= _bitArraySize;
        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearCachedData()
    {
        _cachedTopLeftChunkQuery = new Point(-256, -256);
        _cachedBottomRightChunkQuery = _cachedTopLeftChunkQuery;
    }

    public void Dispose()
    {
        var cachedQueryScratchSpaceHandle = (nint)_cachedQueryScratchSpacePointer;
        var allBitsHandle = (nint)_allBitsPointer;
        var previousItemPositionsHandle = (nint)_previousItemPositionsPointer;

        if (cachedQueryScratchSpaceHandle != nint.Zero)
            Marshal.FreeHGlobal(cachedQueryScratchSpaceHandle);
        if (allBitsHandle != nint.Zero)
            Marshal.FreeHGlobal(allBitsHandle);
        if (previousItemPositionsHandle != nint.Zero)
            Marshal.FreeHGlobal(previousItemPositionsHandle);
    }

    private enum ChunkOperationType
    {
        Add,
        Remove
    }
}
