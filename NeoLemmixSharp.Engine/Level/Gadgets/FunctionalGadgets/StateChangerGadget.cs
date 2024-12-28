using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class StateChangerGadget : GadgetBase, ISimpleGadget
{
    private readonly HitBoxGadget _gadget;
    private readonly int _newState;

    private bool _previousSignal;
    private bool _signal;

    private SimpleGadgetRenderer _renderer;

    public override SimpleGadgetRenderer Renderer => _renderer;

    public StateChangerGadget(
        int id,
        HitBoxGadget gadget,
        int newState)
        : base(id)
    {
        _gadget = gadget;
        _newState = newState;

        RegisterInput(new StateChangerGadgetInput("Input", this));
    }

    public override void Tick()
    {
        if (_signal && !_previousSignal)
        {
            _gadget.SetNextState(_newState);
        }

        _previousSignal = _signal;
    }

    private sealed class StateChangerGadgetInput : IGadgetInput
    {
        private readonly StateChangerGadget _gadget;
        public string InputName { get; }

        public StateChangerGadgetInput(string inputName, StateChangerGadget gadget)
        {
            InputName = inputName;
            _gadget = gadget;
        }

        public void OnRegistered()
        {
        }

        public void ReactToSignal(bool signal)
        {
            _gadget._signal = signal;
        }
    }

    SimpleGadgetRenderer ISimpleGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
}
