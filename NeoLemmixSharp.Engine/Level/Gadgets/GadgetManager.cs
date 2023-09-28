using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager : ISimpleHasher<HitBoxGadget>
{
    private readonly GadgetBase[] _allGadgets;
    private readonly SpacialHashGrid<HitBoxGadget> _gadgetPositionHelper;

    public ReadOnlySpan<GadgetBase> AllGadgets => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        _allGadgets.ValidateUniqueIds();
        Array.Sort(_allGadgets, IdEquatableItemHelperMethods.Compare);

        _gadgetPositionHelper = new SpacialHashGrid<HitBoxGadget>(
            this,
            ChunkSizeType.ChunkSize64,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialise()
    {
        foreach (var gadget in _allGadgets.OfType<HitBoxGadget>())
        {
            _gadgetPositionHelper.AddItem(gadget);
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LargeSimpleSet<HitBoxGadget> GetAllGadgetsForPosition(LevelPosition levelPosition)
    {
        return _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition);
    }

    [Pure]
    public LargeSimpleSet<HitBoxGadget> GetAllGadgetsAtLemmingPosition(Lemming lemming)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        return _gadgetPositionHelper.GetAllItemsNearRegion(levelPositionPair);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LargeSimpleSet<HitBoxGadget> GetAllItemsNearRegion(LevelPositionPair levelRegion)
    {
        return _gadgetPositionHelper.GetAllItemsNearRegion(levelRegion);
    }

    [Pure]
    public bool HasGadgetOfTypeAtPosition(LevelPosition levelPosition, GadgetType gadgetType)
    {
        var gadgetSet = _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition);

        foreach (var gadget in gadgetSet)
        {
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

        var gadgetSet = _gadgetPositionHelper.GetAllItemsNearRegion(levelPositionPair);

        foreach (var gadget in gadgetSet)
        {
            if (gadget.Type == gadgetType && (gadget.MatchesPosition(anchorPixel) || gadget.MatchesPosition(footPixel)))
                return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateGadgetPosition(HitBoxGadget gadget)
    {
        _gadgetPositionHelper.UpdateItemPosition(gadget);
    }

    int ISimpleHasher<HitBoxGadget>.NumberOfItems => _allGadgets.Length;
    int ISimpleHasher<HitBoxGadget>.Hash(HitBoxGadget item) => item.Id;
    HitBoxGadget ISimpleHasher<HitBoxGadget>.UnHash(int index) => (HitBoxGadget)_allGadgets[index];
}