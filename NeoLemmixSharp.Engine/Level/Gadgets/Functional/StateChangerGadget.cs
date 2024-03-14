using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class StateChangerGadget : GadgetBase, IReactiveGadget
{
    private readonly int _newState;

    private bool _previousSignal;
    private bool _signal;

    public StatefulGadget Gadget { get; }

    public override GadgetBehaviour GadgetBehaviour => FunctionalGadgetBehaviour.Instance;
    public override Orientation Orientation => DownOrientation.Instance;

    public IGadgetInput Input { get; }

    public StateChangerGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        IGadgetRenderer? renderer,
        StatefulGadget gadget,
        int newState)
        : base(id, gadgetBounds, renderer)
    {
        Gadget = gadget;
        _newState = newState;

        Input = new StateChangerGadgetInput("Input", this);
    }

    public override void Tick()
    {
        if (_signal && !_previousSignal)
        {
            Gadget.SetNextState(_newState);
        }

        _previousSignal = _signal;
    }

    public IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, Input.InputName))
            return Input;
        return null;
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
}