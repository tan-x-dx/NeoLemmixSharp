using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager : IComparer<GadgetBase>
{
    private const int ChunkSizeBitShift = 6;
    private const int ChunkSize = 1 << ChunkSizeBitShift;
    private const int ChunkSizeBitMask = ChunkSize - 1;

    private readonly GadgetBase[] _allGadgets;
    private readonly LargeBitArray?[] _gadgetChunks;
    private readonly LargeBitArray _indicesOfGadgetChunks;

    private readonly LargeBitArray _indicesOfGadgetChunksScratchSpace;

    private readonly int _totalNumberOfGadgets;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    public ReadOnlySpan<GadgetBase> AllGadgets => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        Array.Sort(_allGadgets, this);
        _allGadgets.ValidateUniqueIds();

        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;

        _totalNumberOfGadgets = allGadgets.Length;
        _numberOfHorizontalChunks = (_horizontalBoundaryBehaviour.LevelWidth + ChunkSizeBitMask) >> ChunkSizeBitShift;
        _numberOfVerticalChunks = (_verticalBoundaryBehaviour.LevelHeight + ChunkSizeBitMask) >> ChunkSizeBitShift;

        _gadgetChunks = new LargeBitArray[_numberOfHorizontalChunks * _numberOfVerticalChunks];
        _indicesOfGadgetChunks = new LargeBitArray(_gadgetChunks.Length);
        _indicesOfGadgetChunksScratchSpace = new LargeBitArray(_gadgetChunks.Length);

        foreach (var gadget in allGadgets)
        {
            UpdateGadgetPosition(gadget);
        }
    }

    [Pure]
    public LargeBitArray.Enumerator GetAllGadgetIdsForPosition(LevelPosition levelPosition)
    {
        var shiftX = levelPosition.X >> ChunkSizeBitShift;
        var shiftY = levelPosition.Y >> ChunkSizeBitShift;

        if (shiftX < 0 || shiftX >= _numberOfHorizontalChunks ||
            shiftY < 0 || shiftY >= _numberOfVerticalChunks)
            return new LargeBitArray.Enumerator();

        var index = _numberOfHorizontalChunks * shiftY + shiftX;
        var gadgetChunk = _gadgetChunks[index];
        return gadgetChunk?.GetEnumerator() ?? new LargeBitArray.Enumerator();
    }

    [Pure]
    public bool HasGadgetOfTypeAtPosition(LevelPosition levelPosition, GadgetType gadgetType)
    {
        var allGadgets = AllGadgets;
        var idEnumerator = GetAllGadgetIdsForPosition(levelPosition);

        while (idEnumerator.MoveNext())
        {
            var gadgetId = idEnumerator.Current;
            var gadget = allGadgets[gadgetId];

            if (gadget.Type != gadgetType)
                continue;
            if (gadget.MatchesPosition(levelPosition))
                return true;
        }

        return false;
    }

    public void UpdateGadgetPosition(int gadgetId)
    {
        var gadget = _allGadgets[gadgetId];
        UpdateGadgetPosition(gadget);
    }

    public void UpdateGadgetPosition(GadgetBase gadget)
    {
        if (!gadget.CaresAboutLemmingInteraction)
            return;

        var topLeft = NormalisePosition(gadget.GadgetBounds.TopLeft);

        var topLeftShiftX = topLeft.X >> ChunkSizeBitShift;
        var topLeftShiftY = topLeft.Y >> ChunkSizeBitShift;
        topLeftShiftX = Math.Clamp(topLeftShiftX, 0, _numberOfHorizontalChunks - 1);
        topLeftShiftY = Math.Clamp(topLeftShiftY, 0, _numberOfVerticalChunks - 1);

        var bottomRight = NormalisePosition(gadget.GadgetBounds.BottomRight);

        var bottomRightShiftX = bottomRight.X >> ChunkSizeBitShift;
        var bottomRightShiftY = bottomRight.Y >> ChunkSizeBitShift;
        bottomRightShiftX = Math.Clamp(bottomRightShiftX, 0, _numberOfHorizontalChunks - 1);
        bottomRightShiftY = Math.Clamp(bottomRightShiftY, 0, _numberOfVerticalChunks - 1);

        WriteToGadgetChunkScratchSpace(topLeftShiftX, topLeftShiftY, bottomRightShiftX, bottomRightShiftY);

        foreach (var gadgetChunkIndex in _indicesOfGadgetChunks)
        {
            var gadgetChunk = _gadgetChunks[gadgetChunkIndex]!;
            gadgetChunk.ClearBit(gadget.Id);
        }

        foreach (var gadgetChunkIndex in _indicesOfGadgetChunksScratchSpace)
        {
            ref var gadgetChunk = ref _gadgetChunks[gadgetChunkIndex];
            gadgetChunk ??= new LargeBitArray(_totalNumberOfGadgets);
            _indicesOfGadgetChunks.SetBit(gadgetChunkIndex);

            gadgetChunk.SetBit(gadget.Id);
        }
    }

    private void WriteToGadgetChunkScratchSpace(int ax, int ay, int bx, int by)
    {
        _indicesOfGadgetChunksScratchSpace.Clear();

        // Is there a more elegant way to deal with all of these cases?

        if (ax <= bx &&
            ay <= by)
        {
            for (var x = ax; x <= bx; x++)
            {
                for (var y = ay; y <= by; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
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
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
                }

                for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
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
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
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
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
                }
            }

            for (var x = _numberOfHorizontalChunks - 1; x >= bx; x--)
            {
                for (var y = 0; y <= ay; y++)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
                }

                for (var y = _numberOfVerticalChunks - 1; y >= by; y--)
                {
                    var index = _numberOfHorizontalChunks * y + x;
                    _indicesOfGadgetChunksScratchSpace.SetBit(index);
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

    int IComparer<GadgetBase>.Compare(GadgetBase? x, GadgetBase? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;
        return x.Id.CompareTo(y.Id);
    }
}