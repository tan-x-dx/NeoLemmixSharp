using NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.States;

public sealed class GadgetState
{
    private readonly int _numberOfAnimationFrames;
    private readonly IGadgetBehaviour[] _interactionActions;
    private readonly GadgetOutput _stateSelectedOutput;

    public ReadOnlySpan<IGadgetBehaviour> Actions => new(_interactionActions);

    public int AnimationFrame { get; private set; }

    public GadgetState(
        int numberOfAnimationFrames,
        IGadgetBehaviour[] interactionActions,
        GadgetOutput stateSelectedOutput)
    {
        _numberOfAnimationFrames = numberOfAnimationFrames;
        _interactionActions = interactionActions;
        _stateSelectedOutput = stateSelectedOutput;
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
            AnimationFrame = 0;
        }
    }

    public void OnTransitionFrom()
    {
        _stateSelectedOutput.SetSignal(false);
    }
}