using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
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

    private readonly uint* _allBitsPointer = default;
    private readonly uint* _cachedQueryScratchSpacePointer = default;
    private readonly RectangularRegion* _previousItemPositionsPointer = default;
    private readonly int _bitArraySize;

    private readonly int _chunkSizeBitShift;
    private readonly Size _sizeInChunks;

    private Point _cachedTopLeftChunkQuery;
    private Point _cachedBottomRightChunkQuery;
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
        var chunkPointer = PointerForChunk(chunkPosition);
        var chunkSpan = new ReadOnlySpan<uint>(chunkPointer, _bitArraySize);
        var popCount = BitArrayHelpers.GetPopCount(chunkPointer, (uint)_bitArraySize);

        result = new BitArrayEnumerable<TPerfectHasher, T>(_hasher, chunkSpan, popCount);
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
            CacheItemsNearRegion(topLeftChunk, bottomRightChunk, out result);
        }
    }

    private void CacheItemsNearRegion(
        Point topLeftChunk,
        Point bottomRightChunk,
        out BitArrayEnumerable<TPerfectHasher, T> result)
    {
        _cachedTopLeftChunkQuery = topLeftChunk;
        _cachedBottomRightChunkQuery = bottomRightChunk;
        CacheLatestQuery();

        _cachedQueryPopCount = BitArrayHelpers.GetPopCount(_cachedQueryScratchSpacePointer, (uint)_bitArraySize);
        result = new BitArrayEnumerable<TPerfectHasher, T>(
            _hasher,
            new ReadOnlySpan<uint>(_cachedQueryScratchSpacePointer, _bitArraySize),
            _cachedQueryPopCount);
    }

    private void CacheLatestQuery()
    {
        var cacheSpan = new Span<uint>(_cachedQueryScratchSpacePointer, _bitArraySize);
        if (_cachedTopLeftChunkQuery == _cachedBottomRightChunkQuery)
        {
            // Only one chunk -> skip some extra work

            var sourceSpan = new ReadOnlySpan<uint>(PointerForChunk(_cachedTopLeftChunkQuery), _bitArraySize);
            sourceSpan.CopyTo(cacheSpan);
        }
        else
        {
            cacheSpan.Clear();
            EvaluateChunkUnions();
        }
    }

    /// <summary>
    /// Performs a chunk union operation over a rectangle of chunks, defined by the cached chunks.
    /// Updates the scratch space pointer buffer to store the results.
    ///
    /// <para>
    /// The rectangle of chunks begins with the top left cached coordinates 
    /// down to the bottom right cached coordinates, inclusive.
    /// </para>
    /// 
    /// <para>
    /// If the bottom right coordinates are less than their respective top left coordinates, then this method wraps around and continues evaluating from zero.
    /// </para>
    /// </summary>
    private void EvaluateChunkUnions()
    {
        var yMax = 1 + _cachedBottomRightChunkQuery.Y - _cachedTopLeftChunkQuery.Y;
        var xMax = 1 + _cachedBottomRightChunkQuery.X - _cachedTopLeftChunkQuery.X;

        if (_cachedBottomRightChunkQuery.X < _cachedTopLeftChunkQuery.X)
            xMax += _sizeInChunks.W;

        if (_cachedBottomRightChunkQuery.Y < _cachedTopLeftChunkQuery.Y)
            yMax += _sizeInChunks.H;

        var y = yMax;
        var x = xMax;
        var yCoord = _cachedTopLeftChunkQuery.Y;
        var xCoord = _cachedTopLeftChunkQuery.X;

        while (y-- > 0)
        {
            while (x-- > 0)
            {
                EvaluateUnionWithChunk(xCoord, yCoord);

                if (++xCoord == _sizeInChunks.W)
                {
                    xCoord = 0;
                }
            }

            x = xMax;
            xCoord = _cachedTopLeftChunkQuery.X;

            if (++yCoord == _sizeInChunks.H)
            {
                yCoord = 0;
            }
        }
    }

    private void EvaluateUnionWithChunk(int xCoord, int yCoord)
    {
        var pointer = PointerForChunk(new Point(xCoord, yCoord));
        BitArrayHelpers.UnionWith(_cachedQueryScratchSpacePointer, pointer, (uint)_bitArraySize);
    }

    public void AddItem(T item)
    {
        if (!_allTrackedItems.Add(item))
            throw new InvalidOperationException("Already tracking item!");

        var currentBounds = item.CurrentBounds;
        var topLeftChunk = GetTopLeftChunkForRegion(currentBounds);
        var bottomRightChunk = GetBottomRightChunkForRegion(currentBounds);

        ClearCachedData();

        RegisterItemPosition(item, topLeftChunk, bottomRightChunk, GetPreviousBoundsForItem(item));
    }

    public void UpdateItemPosition(T item)
    {
        if (!_allTrackedItems.Contains(item))
            throw new InvalidOperationException("Item not registered!");

        var currentBounds = item.CurrentBounds;
        var currentTopLeftChunk = GetTopLeftChunkForRegion(currentBounds);
        var currentBottomRightChunk = GetBottomRightChunkForRegion(currentBounds);

        RectangularRegion* previousBoundsPointer = GetPreviousBoundsForItem(item);
        var previousTopLeftChunk = GetTopLeftChunkForRegion(*previousBoundsPointer);
        var previousBottomRightChunk = GetBottomRightChunkForRegion(*previousBoundsPointer);

        if (currentTopLeftChunk == previousTopLeftChunk &&
            currentBottomRightChunk == previousBottomRightChunk)
            return;

        ClearCachedData();

        DeregisterItemPosition(item, previousTopLeftChunk, previousBottomRightChunk);
        RegisterItemPosition(item, currentTopLeftChunk, currentBottomRightChunk, previousBoundsPointer);
    }

    private void RegisterItemPosition(T item, Point topLeftChunk, Point bottomRightChunk, RectangularRegion* previousBounds)
    {
        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work
            *previousBounds = new RectangularRegion(topLeftChunk);
            var pointer = PointerForChunk(topLeftChunk);
            var hash = _hasher.Hash(item);
            BitArrayHelpers.SetBit(pointer, hash);
        }
        else
        {
            var size = new Size(
                1 + bottomRightChunk.X - topLeftChunk.X,
                1 + bottomRightChunk.Y - topLeftChunk.Y);
            *previousBounds = new RectangularRegion(topLeftChunk, size);
            ModifyChunks(item, ChunkOperationType.Add, topLeftChunk, bottomRightChunk);
        }
    }

    public void RemoveItem(T item)
    {
        if (!_allTrackedItems.Remove(item))
            throw new InvalidOperationException("Not tracking item!");

        var currentBounds = item.CurrentBounds;
        var currentTopLeftChunk = GetTopLeftChunkForRegion(currentBounds);
        var currentBottomRightChunk = GetBottomRightChunkForRegion(currentBounds);

        DeregisterItemPosition(item, currentTopLeftChunk, currentBottomRightChunk);

        RectangularRegion* previousBounds = GetPreviousBoundsForItem(item);
        var previousTopLeftChunk = GetTopLeftChunkForRegion(*previousBounds);
        var previousBottomRightChunk = GetBottomRightChunkForRegion(*previousBounds);

        ClearCachedData();

        if (currentTopLeftChunk == previousTopLeftChunk &&
            currentBottomRightChunk == previousBottomRightChunk)
            return;

        DeregisterItemPosition(item, previousTopLeftChunk, previousBottomRightChunk);
        *previousBounds = new RectangularRegion();
    }

    private void DeregisterItemPosition(T item, Point topLeftChunk, Point bottomRightChunk)
    {
        if (topLeftChunk == bottomRightChunk)
        {
            // Only one chunk -> skip some extra work

            var pointer = PointerForChunk(topLeftChunk);
            var hash = _hasher.Hash(item);
            BitArrayHelpers.ClearBit(pointer, hash);
        }
        else
        {
            ModifyChunks(item, ChunkOperationType.Remove, topLeftChunk, bottomRightChunk);
        }
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
    /// <para>
    /// The rectangle of chunks begins with the top left coordinates of <paramref name="chunkA" />
    /// down to the bottom right coordinates of <paramref name="chunkB" /> inclusive.
    /// </para>
    /// 
    /// <para>
    /// If the <paramref name="chunkB" /> coordinates are less than their respective <paramref name="chunkA" /> coordinates,
    /// then this method wraps around and continues evaluating from zero.
    /// </para>
    /// </summary>
    /// <param name="item">An item to use in part of these chunk operations.</param>
    /// <param name="chunkOperationType">The chunk operation to perform</param>
    /// <param name="chunkA">The top left position.</param>
    /// <param name="chunkB">The bottom right position.</param>
    private void ModifyChunks(T item, ChunkOperationType chunkOperationType, Point chunkA, Point chunkB)
    {
        var yMax = 1 + chunkB.Y - chunkA.Y;
        var xMax = 1 + chunkB.X - chunkA.X;

        if (chunkB.X < chunkA.X)
            xMax += _sizeInChunks.W;

        if (chunkB.Y < chunkA.Y)
            yMax += _sizeInChunks.H;

        var y = yMax;
        var x = xMax;
        var yCoord = chunkA.Y;
        var xCoord = chunkA.X;

        while (y-- > 0)
        {
            while (x-- > 0)
            {
                ModifyChunkPosition(item, chunkOperationType, xCoord, yCoord);

                if (++xCoord == _sizeInChunks.W)
                {
                    xCoord = 0;
                }
            }

            x = xMax;
            xCoord = chunkA.X;

            if (++yCoord == _sizeInChunks.H)
            {
                yCoord = 0;
            }
        }
    }

    private void ModifyChunkPosition(T item, ChunkOperationType chunkOperationType, int xCoord, int yCoord)
    {
        var chunkPosition = new Point(xCoord, yCoord);
        var pointer = PointerForChunk(chunkPosition);
        var hash = _hasher.Hash(item);

        if (chunkOperationType == ChunkOperationType.Add)
        {
            BitArrayHelpers.SetBit(pointer, hash);
        }
        else
        {
            BitArrayHelpers.ClearBit(pointer, hash);
        }
    }

    private RectangularRegion* GetPreviousBoundsForItem(T item)
    {
        int offset = _hasher.Hash(item);
        RectangularRegion* pointer = _previousItemPositionsPointer + offset;

        return pointer;
    }

    [Pure]
    private uint* PointerForChunk(Point pos)
    {
        int offset = _sizeInChunks.GetIndexOfPoint(pos);
        offset *= _bitArraySize;
        uint* pointer = _allBitsPointer + offset;

        return pointer;
    }

    private void ClearCachedData()
    {
        _cachedTopLeftChunkQuery = new Point(-256, -256);
        _cachedBottomRightChunkQuery = _cachedTopLeftChunkQuery;

        new Span<uint>(_cachedQueryScratchSpacePointer, _bitArraySize).Clear();
        _cachedQueryPopCount = 0;
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
