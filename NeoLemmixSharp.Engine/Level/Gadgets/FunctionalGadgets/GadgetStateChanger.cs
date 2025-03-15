using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetStateChanger : GadgetBase
{
    private readonly AnimationController _inactiveAnimationController;
    private readonly AnimationController _activeAnimationController;
    private readonly HitBoxGadget _gadget;

    private readonly int _newState;

    private bool _previousSignal;
    private bool _signal;

    public GadgetStateChanger(
        AnimationController inactiveAnimationController,
        AnimationController activeAnimationController,
        string inputName,
        HitBoxGadget gadget,
        int newState)
        : base(1)
    {
        _inactiveAnimationController = inactiveAnimationController;
        _activeAnimationController = activeAnimationController;
        CurrentAnimationController = _inactiveAnimationController;

        _gadget = gadget;
        _newState = newState;

        RegisterInput(new StateChangerGadgetInput(inputName, this));
    }

    public override void Tick()
    {
        CurrentAnimationController.Tick();

        if (_signal && !_previousSignal)
        {
            _gadget.SetNextState(_newState);
        }

        _previousSignal = _signal;
    }

    private sealed class StateChangerGadgetInput : GadgetInput
    {
        private readonly GadgetStateChanger _gadget;

        public StateChangerGadgetInput(string inputName, GadgetStateChanger gadget)
            : base(inputName)
        {
            _gadget = gadget;
        }

        public override void ReactToSignal(bool signal)
        {
            _gadget._signal = signal;

            _gadget.CurrentAnimationController = signal
                ? _gadget._activeAnimationController
                : _gadget._inactiveAnimationController;
        }
    }
}
