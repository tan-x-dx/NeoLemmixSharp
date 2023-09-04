using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public sealed class ChunkManager<T> : ISimpleHasher<T>, IComparer<T>
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private const int ChunkSizeBitShift = 6;
    private const int ChunkSize = 1 << ChunkSizeBitShift;
    private const int ChunkSizeBitMask = ChunkSize - 1;

    private readonly T[] _allItems;
    private readonly LargeSimpleSet<T>?[] _itemChunks;
    private readonly LargeBitArray _indicesOfItemChunks;

    private readonly LargeBitArray _indicesOfItemChunksScratchSpace;

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    public ChunkManager(
        T[] allItems,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allItems = allItems;
        Array.Sort(_allItems, this);
        _allItems.ValidateUniqueIds();

        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _numberOfHorizontalChunks = (_horizontalBoundaryBehaviour.LevelWidth + ChunkSizeBitMask) >> ChunkSizeBitShift;
        _numberOfVerticalChunks = (_verticalBoundaryBehaviour.LevelHeight + ChunkSizeBitMask) >> ChunkSizeBitShift;

        _itemChunks = new LargeSimpleSet<T>[_numberOfHorizontalChunks * _numberOfVerticalChunks];
        _indicesOfItemChunks = new LargeBitArray(_itemChunks.Length);
        _indicesOfItemChunksScratchSpace = new LargeBitArray(_itemChunks.Length);
    }

    [Pure]
    public LargeSimpleSet<T>.Enumerator GetAllItemIdsForPosition(LevelPosition levelPosition)
    {
        var shiftX = levelPosition.X >> ChunkSizeBitShift;
        var shiftY = levelPosition.Y >> ChunkSizeBitShift;

        if (shiftX < 0 || shiftX >= _numberOfHorizontalChunks ||
            shiftY < 0 || shiftY >= _numberOfVerticalChunks)
            return new LargeSimpleSet<T>.Enumerator();

        var index = _numberOfHorizontalChunks * shiftY + shiftX;
        var itemChunk = _itemChunks[index];
        return itemChunk?.GetEnumerator() ?? new LargeSimpleSet<T>.Enumerator();
    }

    public void UpdateItemPosition(int itemId)
    {
        var item = _allItems[itemId];
        UpdateItemPosition(item);
    }

    public void UpdateItemPosition(T item)
    {
        var topLeft = NormalisePosition(item.TopLeftPixel);

        var topLeftShiftX = topLeft.X >> ChunkSizeBitShift;
        var topLeftShiftY = topLeft.Y >> ChunkSizeBitShift;
        topLeftShiftX = Math.Clamp(topLeftShiftX, 0, _numberOfHorizontalChunks - 1);
        topLeftShiftY = Math.Clamp(topLeftShiftY, 0, _numberOfVerticalChunks - 1);

        var bottomRight = NormalisePosition(item.BottomRightPixel);

        var bottomRightShiftX = bottomRight.X >> ChunkSizeBitShift;
        var bottomRightShiftY = bottomRight.Y >> ChunkSizeBitShift;
        bottomRightShiftX = Math.Clamp(bottomRightShiftX, 0, _numberOfHorizontalChunks - 1);
        bottomRightShiftY = Math.Clamp(bottomRightShiftY, 0, _numberOfVerticalChunks - 1);

        WriteToItemChunkScratchSpace(topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);

        foreach (var itemChunkIndex in _indicesOfItemChunks)
        {
            var itemChunk = _itemChunks[itemChunkIndex]!;
            itemChunk.Remove(item);
        }

        foreach (var itemChunkIndex in _indicesOfItemChunksScratchSpace)
        {
            ref var itemChunk = ref _itemChunks[itemChunkIndex];
            itemChunk ??= new LargeSimpleSet<T>(this);
            _indicesOfItemChunks.SetBit(itemChunkIndex);

            itemChunk.Add(item);
        }
    }

    private void WriteToItemChunkScratchSpace(int ax, int ay, int bx, int by)
    {
        _indicesOfItemChunksScratchSpace.Clear();

        // Is there a more elegant way to deal with all of these cases?

        if (ax <= bx &&
            ay <= by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = ay; y <= by; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }
        }
        else if (ax > bx && ay <= by)
        {
            for (var y = ay; y <= by; y++)
            {
                for (var x = 0; x <= ax; x++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }

                for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }
        }
        else if (ax <= bx && ay > by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }
        }
        else
        {
            for (var x = 0; x <= ax; x++)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }

            for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfItemChunksScratchSpace.SetBit(index);
                }
            }
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        return new LevelPosition(
            _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X),
            _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y));
    }

    bool IEquatable<ISimpleHasher<T>>.Equals(ISimpleHasher<T>? other) => ReferenceEquals(this, other);
    int ISimpleHasher<T>.NumberOfItems => _allItems.Length;
    int ISimpleHasher<T>.Hash(T item) => item.Id;
    T ISimpleHasher<T>.Unhash(int index) => _allItems[index];
    int IComparer<T>.Compare(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;
        return x.Id.CompareTo(y.Id);
    }
}