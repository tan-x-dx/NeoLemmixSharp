using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetStateChanger : FunctionalGadget<GadgetStateChanger.StateChangerGadgetLinkInput>
{
    private readonly HitBoxGadget _gadget;

    private readonly int _newState;

    private bool _previousSignal;
    private bool _signal;

    public GadgetStateChanger(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        GadgetInputName inputName,
        HitBoxGadget gadget,
        int newState)
        : base(gadgetName, states, initialStateIndex, 1)
    {
        _gadget = gadget;
        _newState = newState;

        RegisterInput(new StateChangerGadgetLinkInput(inputName, this));
    }

    protected override void OnTick()
    {
        if (_signal && !_previousSignal)
        {
            _gadget.SetNextState(_newState);
        }

        _previousSignal = _signal;
    }

    protected override void OnChangeStates() { }

    public sealed class StateChangerGadgetLinkInput : GadgetLinkInput
    {
        private readonly GadgetStateChanger _gadget;

        public StateChangerGadgetLinkInput(GadgetInputName inputName, GadgetStateChanger gadget)
            : base(inputName)
        {
            _gadget = gadget;
        }

        public override void ReactToSignal(bool signal)
        {
            _gadget._signal = signal;

            //    _gadget.CurrentAnimationController = signal
            //        ? _gadget._activeAnimationController
            //       : _gadget._inactiveAnimationController;
        }
    }
}
