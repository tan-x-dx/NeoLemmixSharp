using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public unsafe sealed class SpacialHashGrid<TPerfectHasher, TBuffer, T> : IDisposable
    where TPerfectHasher : class, IBitBufferCreator<TBuffer, T>
    where TBuffer : struct, IBitBuffer
    where T : class, IRectangularBounds
{
    private readonly TPerfectHasher _hasher;
    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly BitArraySet<TPerfectHasher, TBuffer, T> _allTrackedItems;

    private readonly uint* _allBitsPointer;
    private readonly uint* _cachedQueryScratchSpacePointer;
    private readonly RectangularRegion* _previousItemPositionsPointer;
    private readonly int _bitArraySize;

    private readonly int _chunkSizeBitShift;
    private readonly Size _sizeInChunks;

    private Point _cachedTopLeftChunkQuery = new(-256, -256);
    private Point _cachedBottomRightChunkQuery = new(-256, -256);
    private int _cachedQueryPopCount;

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
        _allTrackedItems = new BitArraySet<TPerfectHasher, TBuffer, T>(_hasher);

        _chunkSizeBitShift = chunkSize.GetChunkSizeBitShift();
        var chunkSizeBitMask = (1 << _chunkSizeBitShift) - 1;

        _sizeInChunks = new Size(
            (horizontalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >>> _chunkSizeBitShift,
            (verticalBoundaryBehaviour.LevelLength + chunkSizeBitMask) >>> _chunkSizeBitShift);

        _allBitsPointer = (uint*)Marshal.AllocHGlobal(AllBitsSize * sizeof(uint));
        _cachedQueryScratchSpacePointer = (uint*)Marshal.AllocHGlobal(_bitArraySize * sizeof(uint));
        _previousItemPositionsPointer = (RectangularRegion*)Marshal.AllocHGlobal(_hasher.NumberOfItems * sizeof(RectangularRegion));

        Clear();
    }

    [Pure]
    public bool IsEmpty => _allTrackedItems.Count == 0;

    [Pure]
    private int AllBitsSize => _bitArraySize * _sizeInChunks.Area();

    [Pure]
    public bool IsTrackingItem(T item) => _allTrackedItems.Contains(item);

    [Pure]
    public BitArrayEnumerable<TPerfectHasher, T> GetAllTrackedItems() => _allTrackedItems.AsEnumerable();

    public void Clear()
    {
        _allTrackedItems.Clear();
        new Span<uint>(_allBitsPointer, AllBitsSize).Clear();
        new Span<RectangularRegion>(_previousItemPositionsPointer, _hasher.NumberOfItems).Clear();
        ClearCachedData();
    }

    public void GetAllItemsNearPosition(
        Point levelPosition,
        out BitArrayEnumerable<TPerfectHasher, T> result)
    {
        if (IsEmpty)
        {
            result = BitArrayEnumerable<TPerfectHasher, T>.Empty;
            return;
        }

        var chunkPosition = new Point(levelPosition.X >> _chunkSizeBitShift, levelPosition.Y >> _chunkSizeBitShift);

        if (_sizeInChunks.EncompassesPoint(chunkPosition))
        {
            EvaluateItemsNearPosition(chunkPosition, out result);
        }
        else
        {
            result = BitArrayEnumerable<TPerfectHasher, T>.Empty;
        }
    }

    private void EvaluateItemsNearPosition(Point chunkPosition, out BitArrayEnumerable<TPerfectHasher, T> result)
    {
        var sourceSpan = ReadOnlySpanForChunk(chunkPosition);
        var scratchSpaceSpan = new Span<uint>(_cachedQueryScratchSpacePointer, _bitArraySize);
        sourceSpan.CopyTo(scratchSpaceSpan);
        _cachedQueryPopCount = BitArrayHelpers.GetPopCount(_cachedQueryScratchSpacePointer, (uint)_bitArraySize);

        result = new BitArrayEnumerable<TPerfectHasher, T>(_hasher, scratchSpaceSpan, _cachedQueryPopCount);
    }

    /// <summary>
    /// Gets all items that overlap with the input region.
    /// </summary>
    /// <param name="levelRegion">The region to evaluate chunks from</param>
    /// <param name="result">The enumerable</param>
    /// <returns>An enumerable for items within the region</returns>
    public void GetAllItemsNearRegion(
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
            result = new BitArrayEnumerable<TPerfectHasher, T>(_hasher, new ReadOnlySpan<uint>(_cachedQueryScratchSpacePointer, _bitArraySize), _cachedQueryPopCount);
        }
        else
        {
            EvaluateItemsNearRegion(topLeftChunk, bottomRightChunk, out result);
        }
    }

    private void EvaluateItemsNearRegion(
        Point topLeftChunk,
        Point bottomRightChunk,
        out BitArrayEnumerable<TPerfectHasher, T> result)
    {
        var scratchSpaceSpan = new Span<uint>(_cachedQueryScratchSpacePointer, _bitArraySize);
        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work

            var sourceSpan = ReadOnlySpanForChunk(topLeftChunk);
            sourceSpan.CopyTo(scratchSpaceSpan);
        }
        else
        {
            scratchSpaceSpan.Clear();
            EvaluateChunkUnions(topLeftChunk, bottomRightChunk);
        }

        _cachedTopLeftChunkQuery = topLeftChunk;
        _cachedBottomRightChunkQuery = bottomRightChunk;
        _cachedQueryPopCount = BitArrayHelpers.GetPopCount(_cachedQueryScratchSpacePointer, (uint)_bitArraySize);
        result = new BitArrayEnumerable<TPerfectHasher, T>(_hasher, scratchSpaceSpan, _cachedQueryPopCount);
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
    /// Performs a chunk union operation over a rectangle of chunks, using the scratch space pointer buffer to store the results.
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
    private void EvaluateChunkUnions(Point chunkA, Point chunkB)
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
                BitArrayHelpers.UnionWith(_cachedQueryScratchSpacePointer, PointerForChunk(new Point(x1, y1)), (uint)_bitArraySize);

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
        RectangularRegion* pointer = _previousItemPositionsPointer + index;

        return ref *pointer;
    }

    [Pure]
    private Span<uint> SpanForChunk(Point pos)
    {
        var index = IndexForChunk(pos);
        uint* pointer = _allBitsPointer + index;

        return new Span<uint>(pointer, _bitArraySize);
    }

    [Pure]
    private uint* PointerForChunk(Point pos)
    {
        var index = IndexForChunk(pos);
        uint* pointer = _allBitsPointer + index;

        return pointer;
    }

    [Pure]
    private ReadOnlySpan<uint> ReadOnlySpanForChunk(Point pos)
    {
        var index = IndexForChunk(pos);
        uint* pointer = _allBitsPointer + index;

        return new ReadOnlySpan<uint>(pointer, _bitArraySize);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexForChunk(Point pos)
    {
        var index = _sizeInChunks.GetIndexOfPoint(pos);
        index *= _bitArraySize;
        return index;
    }

    private void ClearCachedData()
    {
        _cachedTopLeftChunkQuery = new Point(-256, -256);
        _cachedBottomRightChunkQuery = _cachedTopLeftChunkQuery;

        new Span<uint>(_cachedQueryScratchSpacePointer, _bitArraySize).Clear();
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
