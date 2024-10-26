using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager :
    IPerfectHasher<GadgetBase>,
    IItemManager<GadgetBase>,
    IPerfectHasher<HitBoxGadget>,
    ISnapshotDataConvertible<int>,
    IInitialisable,
    IDisposable
{
    private readonly GadgetBase[] _allGadgets;
    private readonly SpacialHashGrid<GadgetManager, HitBoxGadget> _gadgetPositionHelper;

    public ReadOnlySpan<GadgetBase> AllItems => new(_allGadgets);

    public GadgetManager(
        GadgetBase[] allGadgets,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _allGadgets = allGadgets;
        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<GadgetBase>(allGadgets));
        Array.Sort(_allGadgets, IdEquatableItemHelperMethods.Compare);

        _gadgetPositionHelper = new SpacialHashGrid<GadgetManager, HitBoxGadget>(
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

    public void GetAllGadgetsForPosition(
        Span<uint> scratchSpaceSpan,
        LevelPosition levelPosition,
        out GadgetEnumerable result)
    {
        _gadgetPositionHelper.GetAllItemsNearPosition(
            scratchSpaceSpan,
            levelPosition,
            out result);
    }

    public void GetAllGadgetsAtLemmingPosition(
        Span<uint> scratchSpace,
        Lemming lemming,
        out GadgetEnumerable result)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var lemmingPositionRegion = new Common.Util.LevelRegion(anchorPixel, footPixel);

        _gadgetPositionHelper.GetAllItemsNearRegion(
            scratchSpace,
            lemmingPositionRegion,
            out result);
    }

    public void GetAllItemsNearRegion(
        Span<uint> scratchSpace,
        Common.Util.LevelRegion levelRegion,
        out GadgetEnumerable result)
    {
        _gadgetPositionHelper.GetAllItemsNearRegion(
            scratchSpace,
            levelRegion,
            out result);
    }

    public bool HasGadgetWithBehaviourAtPosition(
        Span<uint> scratchSpaceSpan,
        LevelPosition levelPosition,
        GadgetBehaviour gadgetBehaviour)
    {
        _gadgetPositionHelper.GetAllItemsNearPosition(
            scratchSpaceSpan,
            levelPosition,
            out var gadgetSet);

        foreach (var gadget in gadgetSet)
        {
            if (gadget.GadgetBehaviour == gadgetBehaviour && gadget.MatchesPosition(levelPosition))
                return true;
        }

        return false;
    }

    public bool HasGadgetWithBehaviourAtLemmingPosition(
        Span<uint> scratchSpaceSpan,
        Lemming lemming,
        GadgetBehaviour gadgetBehaviour)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var lemmingPositionRegion = new Common.Util.LevelRegion(anchorPixel, footPixel);

        _gadgetPositionHelper.GetAllItemsNearRegion(
            scratchSpaceSpan,
            lemmingPositionRegion,
            out var gadgetSet);

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

    public void ToSnapshotData(out int snapshotData)
    {
        snapshotData = 0;
    }

    public void SetFromSnapshotData(in int snapshotData)
    {
        _gadgetPositionHelper.Clear();

        var gadgets = AllItems;

        foreach (var gadget in gadgets)
        {
            if (gadget is HitBoxGadget hitBoxGadget)
            {
                _gadgetPositionHelper.AddItem(hitBoxGadget);
            }
        }
    }
}