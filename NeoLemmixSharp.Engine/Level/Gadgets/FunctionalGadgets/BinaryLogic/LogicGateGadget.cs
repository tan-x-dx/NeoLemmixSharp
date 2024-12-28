using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class LogicGateGadget : GadgetBase, ISimpleGadget
{
    private SimpleGadgetRenderer _renderer;

    public sealed override SimpleGadgetRenderer Renderer => _renderer;

    public GadgetOutput Output { get; } = new();

    public LogicGateGadget(int id) : base(id)
    {
    }

    public abstract void EvaluateInputs();

    protected sealed class LogicGateGadgetInput : IGadgetInput
    {
        private readonly LogicGateGadget _gadget;

        public string InputName { get; }
        public bool Signal { get; private set; }

        public LogicGateGadgetInput(string inputName, LogicGateGadget gadget)
        {
            _gadget = gadget;
            InputName = inputName;
        }

        public void OnRegistered()
        {
        }

        public void ReactToSignal(bool signal)
        {
            Signal = signal;
            _gadget.EvaluateInputs();
        }
    }

    SimpleGadgetRenderer ISimpleGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
}
