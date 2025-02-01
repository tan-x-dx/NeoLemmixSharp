using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetStateChanger : GadgetBase, ISimpleRenderGadget
{
    private SimpleGadgetRenderer _renderer;
    private readonly HitBoxGadget _gadget;
    private readonly int _newState;

    private bool _previousSignal;
    private bool _signal;

    public override SimpleGadgetRenderer Renderer => _renderer;

    public GadgetStateChanger(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        string inputName,
        HitBoxGadget gadget,
        int newState)
        : base(id, orientation, gadgetBounds, 1)
    {
        _gadget = gadget;
        _newState = newState;

        RegisterInput(new StateChangerGadgetInput(inputName, this));
    }

    public override void Tick()
    {
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
        }
    }

    SimpleGadgetRenderer ISimpleRenderGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
}
