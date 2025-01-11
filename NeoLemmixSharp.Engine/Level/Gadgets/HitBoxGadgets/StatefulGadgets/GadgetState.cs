﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class GadgetState
{
    private readonly GadgetOutput _stateSelectedOutput = new();
    private readonly LemmingHitBoxFilter[] _lemmingHitBoxFilters;

    public IHitBoxRegion HitBoxRegion { get; }
    public LevelPosition HitBoxOffset { get; }
    public GadgetStateAnimationController AnimationController { get; }

    public ReadOnlySpan<LemmingHitBoxFilter> Filters => new(_lemmingHitBoxFilters);

    public GadgetState(
        GadgetStateAnimationController animationController,
        IHitBoxRegion hitBoxRegion,
        LevelPosition hitBoxOffset,
        LemmingHitBoxFilter[] lemmingHitBoxFilters)
    {
        AnimationController = animationController;
        _lemmingHitBoxFilters = lemmingHitBoxFilters;
        HitBoxRegion = hitBoxRegion;
        HitBoxOffset = hitBoxOffset;
    }

    public void OnTransitionTo()
    {
        AnimationController.OnTransitionTo();
        _stateSelectedOutput.SetSignal(true);
    }

    public void Tick(HitBoxGadget parentGadget)
    {
        var gadgetStateTransitionIndex = AnimationController.Tick();

        if (gadgetStateTransitionIndex != GadgetStateAnimationController.NoGadgetStateTransition)
        {
            parentGadget.SetNextState(gadgetStateTransitionIndex);
        }
    }

    public void OnTransitionFrom()
    {
        _stateSelectedOutput.SetSignal(false);
    }
}
