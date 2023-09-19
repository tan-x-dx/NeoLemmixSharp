using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager : ISimpleHasher<HitBoxGadget>
{
    private readonly GadgetBase[] _allGadgets;
    private readonly PositionHelper<HitBoxGadget> _gadgetPositionHelper;

    public ReadOnlySpan<GadgetBase> AllGadgets => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        _allGadgets.ValidateUniqueIds();
        Array.Sort(_allGadgets, IdEquatableItemHelperMethods.Compare);

        _gadgetPositionHelper = new PositionHelper<HitBoxGadget>(
            this,
            ChunkSizeType.ChunkSize64,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour,
            1);
    }

    public void Initialise()
    {
        foreach (var gadget in _allGadgets.OfType<HitBoxGadget>())
        {
            _gadgetPositionHelper.AddItem(gadget);
        }
    }

    [Pure]
    public LargeSimpleSet<HitBoxGadget>.Enumerator GetAllGadgetsForPosition(LevelPosition levelPosition)
    {
        return _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition);
    }

    [Pure]
    public LargeSimpleSet<HitBoxGadget>.Enumerator GetAllGadgetsAtLemmingPosition(Lemming lemming)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        var topLeftPixel = levelPositionPair.GetTopLeftPosition();
        var bottomRightPixel = levelPositionPair.GetBottomRightPosition();

        return _gadgetPositionHelper.GetAllItemsNearRegion(topLeftPixel, bottomRightPixel);
    }

    [Pure]
    public LargeSimpleSet<HitBoxGadget>.Enumerator GetAllItemsNearRegion(LevelPosition topLeftPixel, LevelPosition bottomRightLevelPosition)
    {
        return _gadgetPositionHelper.GetAllItemsNearRegion(topLeftPixel, bottomRightLevelPosition);
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
        var footPixel = lemming.FootPosition;

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        var topLeftPixel = levelPositionPair.GetTopLeftPosition();
        var bottomRightPixel = levelPositionPair.GetBottomRightPosition();

        var gadgetEnumerator = _gadgetPositionHelper.GetAllItemsNearRegion(topLeftPixel, bottomRightPixel);

        while (gadgetEnumerator.MoveNext())
        {
            var gadget = gadgetEnumerator.Current;

            if (gadget.Type == gadgetType && (gadget.MatchesPosition(anchorPixel) || gadget.MatchesPosition(footPixel)))
                return true;
        }

        return false;
    }

    public void UpdateGadgetPosition(HitBoxGadget gadget)
    {
        _gadgetPositionHelper.UpdateItemPosition(gadget);
    }

    int ISimpleHasher<HitBoxGadget>.NumberOfItems => _allGadgets.Length;
    int ISimpleHasher<HitBoxGadget>.Hash(HitBoxGadget item) => item.Id;
    HitBoxGadget ISimpleHasher<HitBoxGadget>.UnHash(int index) => (HitBoxGadget)_allGadgets[index];
}