using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public sealed class GadgetManager : IComparer<Gadget>
{
    public const int ChunkSizeBitShift = 6;
    public const int ChunkSize = 1 << ChunkSizeBitShift;
    public const int ChunkSizeBitMask = ChunkSize - 1;

    private readonly Gadget[] _allGadgets;
    private readonly LargeBitArray?[] _gadgetChunks;

    private readonly int _totalNumberOfGadgets;
    private readonly int _numberOfHorizontalChunks;
    private readonly int _numberOfVerticalChunks;

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    public ReadOnlySpan<Gadget> AllGadgets => new(_allGadgets);

    public GadgetManager(
        Gadget[] allGadgets,
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

        if (shiftX < 0 ||
            shiftX >= _numberOfHorizontalChunks ||
            shiftY < 0 ||
            shiftY >= _numberOfVerticalChunks)
            return new LargeBitArray.Enumerator();

        var index = _numberOfHorizontalChunks * shiftY + shiftX;
        var gadgetChunk = _gadgetChunks[index];
        return gadgetChunk?.GetEnumerator() ?? new LargeBitArray.Enumerator();
    }

    [Pure]
    public bool HasGadgetOfTypeAtPosition(Lemming lemming, LevelPosition levelPosition, GadgetType gadgetType)
    {
        var allGadgets = AllGadgets;
        var idEnumerator = GetAllGadgetIdsForPosition(levelPosition);

        while (idEnumerator.MoveNext())
        {
            var gadgetId = idEnumerator.Current;
            var gadget = allGadgets[gadgetId];

            if (gadget.Type != gadgetType)
                continue;

            if (gadget.MatchesLemmingAndPosition(lemming, levelPosition))
                return true;
        }

        return false;
    }

    public void UpdateGadgetPosition(Gadget gadget)
    {
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

        for (var x = 0; x < _numberOfHorizontalChunks; x++)
        {
            for (var y = 0; y < _numberOfVerticalChunks; y++)
            {
                var index = _numberOfHorizontalChunks * y + x;
                ref var gadgetChunk = ref _gadgetChunks[index];

                if (x >= topLeftShiftX && x <= bottomRightShiftX &&
                    y >= topLeftShiftY && y <= bottomRightShiftY)
                {
                    gadgetChunk ??= new LargeBitArray(_totalNumberOfGadgets);

                    gadgetChunk.SetBit(gadget.Id);
                }
                else
                {
                    gadgetChunk?.ClearBit(gadget.Id);
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

    int IComparer<Gadget>.Compare(Gadget? x, Gadget? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;
        return x.Id.CompareTo(y.Id);
    }
}