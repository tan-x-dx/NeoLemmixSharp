using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager : IPerfectHasher<GadgetBase>, IItemManager<GadgetBase>, IPerfectHasher<HitBoxGadget>, IInitialisable, IDisposable
{
    private readonly GadgetBase[] _allGadgets;
    private readonly SpacialHashGrid<HitBoxGadget> _gadgetPositionHelper;

    public ReadOnlySpan<GadgetBase> AllItems => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<GadgetBase>(allGadgets));
        Array.Sort(_allGadgets, IdEquatableItemHelperMethods.Compare);

        _gadgetPositionHelper = new SpacialHashGrid<HitBoxGadget>(
            this,
            LevelConstants.GadgetPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
    }

    public int ScratchSpaceSize => _gadgetPositionHelper.ScratchSpaceSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialise()
    {
        foreach (var gadget in _allGadgets)
        {
            if (gadget is HitBoxGadget hitBoxGadget)
            {
                _gadgetPositionHelper.AddItem(hitBoxGadget);
            }
        }
    }

    public void Tick(bool isMajorTick)
    {
        foreach (var gadget in _allGadgets)
        {
            if (isMajorTick)
            {
                gadget.Tick();
            }
        }
    }

    [Pure]
    public void GetAllGadgetsForPosition(LevelPosition levelPosition, out GadgetSet result)
    {
        _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition, out result);
    }

    [Pure]
    public void GetAllGadgetsAtLemmingPosition(Lemming lemming, out GadgetSet result)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        _gadgetPositionHelper.GetAllItemsNearRegion(levelPositionPair, out result);
    }

    [Pure]
    public void GetAllGadgetsAtLemmingPosition(
        Span<uint> scratchSpace,
        Lemming lemming,
        out GadgetSet result)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        _gadgetPositionHelper.GetAllItemsNearRegion(scratchSpace, levelPositionPair, out result);
    }

    [Pure]
    public void GetAllItemsNearRegion(LevelPositionPair levelRegion, out GadgetSet result)
    {
        _gadgetPositionHelper.GetAllItemsNearRegion(levelRegion, out result);
    }

    [Pure]
    public void GetAllItemsNearRegion(
        Span<uint> scratchSpace,
        LevelPositionPair levelRegion,
        out GadgetSet result)
    {
        _gadgetPositionHelper.GetAllItemsNearRegion(scratchSpace, levelRegion, out result);
    }

    [Pure]
    public bool HasGadgetWithBehaviourAtPosition(LevelPosition levelPosition, GadgetBehaviour gadgetBehaviour)
    {
        _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition, out var gadgetSet);

        foreach (var gadget in gadgetSet)
        {
            if (gadget.GadgetBehaviour == gadgetBehaviour && gadget.MatchesPosition(levelPosition))
                return true;
        }

        return false;
    }

    [Pure]
    public bool HasGadgetWithBehaviourAtLemmingPosition(Lemming lemming, GadgetBehaviour gadgetBehaviour)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        _gadgetPositionHelper.GetAllItemsNearRegion(levelPositionPair, out var gadgetSet);

        foreach (var gadget in gadgetSet)
        {
            if (gadget.GadgetBehaviour == gadgetBehaviour && (gadget.MatchesPosition(anchorPixel) || gadget.MatchesPosition(footPixel)))
                return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateGadgetPosition(HitBoxGadget gadget)
    {
        _gadgetPositionHelper.UpdateItemPosition(gadget);
    }

    public int NumberOfItems => _allGadgets.Length;
    int IPerfectHasher<GadgetBase>.Hash(GadgetBase item) => item.Id;
    GadgetBase IPerfectHasher<GadgetBase>.UnHash(int index) => _allGadgets[index];
    int IPerfectHasher<HitBoxGadget>.Hash(HitBoxGadget item) => item.Id;
    HitBoxGadget IPerfectHasher<HitBoxGadget>.UnHash(int index) => (HitBoxGadget)_allGadgets[index];

    public void Dispose()
    {
        Array.Clear(_allGadgets);
        _gadgetPositionHelper.Clear();
    }
}