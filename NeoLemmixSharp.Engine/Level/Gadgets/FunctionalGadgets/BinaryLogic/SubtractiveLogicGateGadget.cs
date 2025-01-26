using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class SubtractiveLogicGateGadget : GadgetBase,
    ILogicGateGadget,
    ISimpleRenderGadget
{
    private SimpleGadgetRenderer _renderer;

    public GadgetOutput Output { get; } = new();
    public LogicGateType Type { get; }
    private bool _shouldTick;

    public sealed override SimpleGadgetRenderer Renderer => _renderer;

    protected SubtractiveLogicGateGadget(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        LogicGateType type,
        int expectedNumberOfInputs)
        : base(id, orientation, gadgetBounds, expectedNumberOfInputs)
    {
        Type = type;
    }

    public sealed override void Tick()
    {
        if (!_shouldTick)
            return;

        _shouldTick = false;
        Output.SetSignal(EvaluateInputs());
    }

    protected abstract bool EvaluateInputs();

    protected sealed class SubtractiveLogicGateGadgetInput : GadgetInput
    {
        private readonly SubtractiveLogicGateGadget _gadget;

        public bool Signal { get; private set; }

        public SubtractiveLogicGateGadgetInput(string inputName, SubtractiveLogicGateGadget gadget)
            : base(inputName)
        {
            _gadget = gadget;
        }

        public override void ReactToSignal(bool signal)
        {
            Signal = signal;
            _gadget._shouldTick = true;
        }
    }

    SimpleGadgetRenderer ISimpleRenderGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
}

public sealed class NotGateGadget : SubtractiveLogicGateGadget
{
    private readonly SubtractiveLogicGateGadgetInput _input;

    public NotGateGadget(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        string inputName)
        : base(id, orientation, gadgetBounds, LogicGateType.NotGate, 1)
    {
        _input = new SubtractiveLogicGateGadgetInput(inputName, this);
        RegisterInput(_input);
    }

    protected override bool EvaluateInputs()
    {
        return !_input.Signal;
    }
}

public sealed class XorGateGadget : SubtractiveLogicGateGadget
{
    private readonly SubtractiveLogicGateGadgetInput _input1;
    private readonly SubtractiveLogicGateGadgetInput _input2;

    public XorGateGadget(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        string input1Name,
        string input2Name)
        : base(id, orientation, gadgetBounds, LogicGateType.XorGate, 2)
    {
        _input1 = new SubtractiveLogicGateGadgetInput(input1Name, this);
        _input2 = new SubtractiveLogicGateGadgetInput(input2Name, this);
        RegisterInput(_input1);
        RegisterInput(_input2);
    }

    protected override bool EvaluateInputs()
    {
        return _input1.Signal != _input2.Signal;
    }
}