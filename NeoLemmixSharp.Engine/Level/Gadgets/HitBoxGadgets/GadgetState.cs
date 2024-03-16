﻿using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class GadgetState
{
    private readonly IGadgetAction[] _onLemmingEnterActions;
    private readonly IGadgetAction[] _onLemmingPresentActions;
    private readonly IGadgetAction[] _onLemmingExitActions;

    private readonly GadgetOutput _stateSelectedOutput = new();

    private StatefulGadget _gadget = null!;

    public HitBox HitBox { get; }

    public ReadOnlySpan<IGadgetAction> OnLemmingEnterActions => new(_onLemmingEnterActions);
    public ReadOnlySpan<IGadgetAction> OnLemmingPresentActions => new(_onLemmingPresentActions);
    public ReadOnlySpan<IGadgetAction> OnLemmingExitActions => new(_onLemmingExitActions);

    public int AnimationFrame { get; private set; }

    public GadgetState(
        IGadgetAction[] onLemmingEnterActions,
        IGadgetAction[] onLemmingPresentActions,
        IGadgetAction[] onLemmingExitActions,
        HitBox hitBox)
    {
        _onLemmingEnterActions = onLemmingEnterActions;
        _onLemmingPresentActions = onLemmingPresentActions;
        _onLemmingExitActions = onLemmingExitActions;

        HitBox = hitBox;
    }

    public void SetGadget(StatefulGadget gadget)
    {
        _gadget = gadget;
    }

    public void OnTransitionTo()
    {
        AnimationFrame = 0;
        _stateSelectedOutput.SetSignal(true);
    }

    public void Tick()
    {
       /* if (AnimationFrame < _numberOfAnimationFrames)
        {
            AnimationFrame++;
        }
        else
        {
            if (_stateTransitionAfterAnimation == -1)
            {
                AnimationFrame = 0;
            }
            else
            {
                _gadget.SetNextState(_stateTransitionAfterAnimation);
            }
        }*/
    }

    public void OnTransitionFrom()
    {
        _stateSelectedOutput.SetSignal(false);
    }
}