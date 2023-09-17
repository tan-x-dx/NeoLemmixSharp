using NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.States;

public sealed class GadgetState
{
    private readonly int _numberOfAnimationFrames;
    private readonly IGadgetBehaviour[] _interactionActions;
    private readonly GadgetOutput _stateSelectedOutput;
    private readonly int _stateTransitionAfterAnimation;

    private StatefulGadget _gadget = null!;

    public ReadOnlySpan<IGadgetBehaviour> Actions => new(_interactionActions);

    public int AnimationFrame { get; private set; }

    public GadgetState(
        int numberOfAnimationFrames,
        IGadgetBehaviour[] interactionActions,
        GadgetOutput stateSelectedOutput,
        int stateTransitionAfterAnimation)
    {
        _numberOfAnimationFrames = numberOfAnimationFrames;
        _interactionActions = interactionActions;
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