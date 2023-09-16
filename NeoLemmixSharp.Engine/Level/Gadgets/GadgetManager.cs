using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager : ISimpleHasher<GadgetBase>
{
    private readonly GadgetBase[] _allGadgets;
    private readonly PositionHelper<GadgetBase> _gadgetPositionHelper;

    public ReadOnlySpan<GadgetBase> AllGadgets => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        _allGadgets.ValidateUniqueIds();
        Array.Sort(_allGadgets, IdEquatableItemHelperMethods.Compare);

        _gadgetPositionHelper = new PositionHelper<GadgetBase>(
            this,
            ChunkSizeType.ChunkSize64,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
    }

    public void Initialise()
    {
        foreach (var gadget in _allGadgets)
        {
            if (gadget.CaresAboutLemmingInteraction)
            {
                _gadgetPositionHelper.AddItem(gadget);
            }
        }
    }

    [Pure]
    public LargeSimpleSet<GadgetBase>.Enumerator GetAllGadgetsForPosition(LevelPosition levelPosition)
    {
        return _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition);
    }

    [Pure]
    public LargeSimpleSet<GadgetBase>.Enumerator GetAllGadgetsAtLemmingPosition(Lemming lemming)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.Orientation.MoveUp(anchorPixel, 1);

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        var topLeftPixel = levelPositionPair.GetTopLeftPosition();
        var bottomRightPixel = levelPositionPair.GetBottomRightPosition();

        return _gadgetPositionHelper.GetItemsNearRegionEnumerator(topLeftPixel, bottomRightPixel);
    }

    [Pure]
    public bool HasGadgetOfTypeAtPosition(LevelPosition levelPosition, GadgetType gadgetType)
    {
        var gadgetEnumerator = _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition);

        while (gadgetEnumerator.MoveNext())
        {
            var gadget = gadgetEnumerator.Current;

            if (gadget.Type == gadgetType && gadget.MatchesPosition(levelPosition))
                return true;
        }

        return false;
    }

    [Pure]
    public bool HasGadgetOfTypeAtLemmingPosition(Lemming lemming, GadgetType gadgetType)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.Orientation.MoveUp(anchorPixel, 1);

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        var topLeftPixel = levelPositionPair.GetTopLeftPosition();
        var bottomRightPixel = levelPositionPair.GetBottomRightPosition();

        var gadgetEnumerator = _gadgetPositionHelper.GetItemsNearRegionEnumerator(topLeftPixel, bottomRightPixel);

        while (gadgetEnumerator.MoveNext())
        {
            var gadget = gadgetEnumerator.Current;

            if (gadget.Type != gadgetType)
                continue;
            if (gadget.MatchesPosition(anchorPixel) || gadget.MatchesPosition(footPixel))
                return true;
        }

        return false;
    }

    public void UpdateGadgetPosition(GadgetBase gadget)
    {
        if (gadget.CaresAboutLemmingInteraction)
        {
            _gadgetPositionHelper.UpdateItemPosition(gadget);
        }
    }

    int ISimpleHasher<GadgetBase>.NumberOfItems => _allGadgets.Length;
    int ISimpleHasher<GadgetBase>.Hash(GadgetBase item) => item.Id;
    GadgetBase ISimpleHasher<GadgetBase>.UnHash(int index) => _allGadgets[index];
}