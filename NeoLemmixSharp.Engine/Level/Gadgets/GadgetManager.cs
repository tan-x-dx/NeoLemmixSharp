﻿using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Updates;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public sealed class GadgetManager : IPerfectHasher<HitBoxGadget>, IDisposable
{
    private readonly GadgetBase[] _allGadgets;
    private readonly SpacialHashGrid<HitBoxGadget> _gadgetPositionHelper;

    public ReadOnlySpan<GadgetBase> AllGadgets => new(_allGadgets);
    
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

    public void Tick(UpdateState updateState, bool isMajorTick)
    {
        if (updateState != UpdateState.FastForward && !isMajorTick)
            return;

        foreach (var gadget in AllGadgets)
        {
            gadget.Tick();
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GadgetSet GetAllGadgetsForPosition(LevelPosition levelPosition)
    {
        return _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition);
    }

    [Pure]
    public GadgetSet GetAllGadgetsAtLemmingPosition(Lemming lemming)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        return _gadgetPositionHelper.GetAllItemsNearRegion(levelPositionPair);
    }

    [Pure]
    public GadgetSet GetAllGadgetsAtLemmingPosition(
        Span<uint> scratchSpace,
        Lemming lemming)
    {
        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        var levelPositionPair = new LevelPositionPair(anchorPixel, footPixel);

        return _gadgetPositionHelper.GetAllItemsNearRegion(scratchSpace, levelPositionPair);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GadgetSet GetAllItemsNearRegion(LevelPositionPair levelRegion)
    {
        return _gadgetPositionHelper.GetAllItemsNearRegion(levelRegion);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GadgetSet GetAllItemsNearRegion(
        Span<uint> scratchSpace,
        LevelPositionPair levelRegion)
    {
        return _gadgetPositionHelper.GetAllItemsNearRegion(scratchSpace, levelRegion);
    }

    [Pure]
    public bool HasGadgetWithBehaviourAtPosition(LevelPosition levelPosition, GadgetBehaviour gadgetBehaviour)
    {
        var gadgetSet = _gadgetPositionHelper.GetAllItemsNearPosition(levelPosition);

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

        var gadgetSet = _gadgetPositionHelper.GetAllItemsNearRegion(levelPositionPair);

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

    int IPerfectHasher<HitBoxGadget>.NumberOfItems => _allGadgets.Length;
    int IPerfectHasher<HitBoxGadget>.Hash(HitBoxGadget item) => item.Id;
    HitBoxGadget IPerfectHasher<HitBoxGadget>.UnHash(int index) => (HitBoxGadget)_allGadgets[index];

    public void Dispose()
    {
        Array.Clear(_allGadgets);
        _gadgetPositionHelper.Clear();
    }
}