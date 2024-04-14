using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class GadgetState
{
    private readonly IGadgetAction[] _onLemmingEnterActions;
    private readonly IGadgetAction[] _onLemmingPresentActions;
    private readonly IGadgetAction[] _onLemmingExitActions;

    private readonly GadgetOutput _stateSelectedOutput = new();

    private StatefulGadget _gadget = null!;

    public GadgetStateAnimationController AnimationController { get; }
    public GadgetBehaviour GadgetBehaviour { get; }
    public HitBox HitBox { get; }

    public ReadOnlySpan<IGadgetAction> OnLemmingEnterActions => new(_onLemmingEnterActions);
    public ReadOnlySpan<IGadgetAction> OnLemmingPresentActions => new(_onLemmingPresentActions);
    public ReadOnlySpan<IGadgetAction> OnLemmingExitActions => new(_onLemmingExitActions);

    public GadgetState(
        GadgetBehaviour gadgetBehaviour,
        IGadgetAction[] onLemmingEnterActions,
        IGadgetAction[] onLemmingPresentActions,
        IGadgetAction[] onLemmingExitActions,
        GadgetStateAnimationController animationController,
        HitBox hitBox)
    {
        GadgetBehaviour = gadgetBehaviour;
        _onLemmingEnterActions = onLemmingEnterActions;
        _onLemmingPresentActions = onLemmingPresentActions;
        _onLemmingExitActions = onLemmingExitActions;
        AnimationController = animationController;
        HitBox = hitBox;
    }

    public void SetGadget(StatefulGadget gadget)
    {
        _gadget = gadget;
    }

    public void OnTransitionTo()
    {
        AnimationController.OnTransitionTo();
        _stateSelectedOutput.SetSignal(true);
    }

    public void Tick()
    {
        var gadgetStateTransitionIndex = AnimationController.Tick();

        if (gadgetStateTransitionIndex != GadgetStateAnimationController.NoGadgetStateTransition)
        {
            _gadget.SetNextState(gadgetStateTransitionIndex);
        }
    }

    public void OnTransitionFrom()
    {
        _stateSelectedOutput.SetSignal(false);
    }
}