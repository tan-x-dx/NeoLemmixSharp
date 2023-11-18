using NeoLemmixSharp.Engine.Level.Gadgets.GadgetBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class GadgetState
{
    private readonly int _numberOfAnimationFrames;

    private readonly IGadgetBehaviour[] _onLemmingEnterActions;
    private readonly IGadgetBehaviour[] _onLemmingPresentActions;
    private readonly IGadgetBehaviour[] _onLemmingExitActions;

    private readonly GadgetOutput _stateSelectedOutput;
    private readonly int _stateTransitionAfterAnimation;

    private StatefulGadget _gadget = null!;

    public ReadOnlySpan<IGadgetBehaviour> OnLemmingEnterActions => new(_onLemmingEnterActions);
    public ReadOnlySpan<IGadgetBehaviour> OnLemmingPresentActions => new(_onLemmingPresentActions);
    public ReadOnlySpan<IGadgetBehaviour> OnLemmingExitActions => new(_onLemmingExitActions);

    public int AnimationFrame { get; private set; }

    public GadgetState(
        int numberOfAnimationFrames,
        IGadgetBehaviour[] onLemmingEnterActions,
        IGadgetBehaviour[] onLemmingPresentActions,
        IGadgetBehaviour[] onLemmingExitActions,
        GadgetOutput stateSelectedOutput,
        int stateTransitionAfterAnimation)
    {
        _numberOfAnimationFrames = numberOfAnimationFrames;

        _onLemmingEnterActions = onLemmingEnterActions;
        _onLemmingPresentActions = onLemmingPresentActions;
        _onLemmingExitActions = onLemmingExitActions;

        _stateSelectedOutput = stateSelectedOutput;
        _stateTransitionAfterAnimation = stateTransitionAfterAnimation;
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
        if (AnimationFrame < _numberOfAnimationFrames)
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
        }
    }

    public void OnTransitionFrom()
    {
        _stateSelectedOutput.SetSignal(false);
    }
}