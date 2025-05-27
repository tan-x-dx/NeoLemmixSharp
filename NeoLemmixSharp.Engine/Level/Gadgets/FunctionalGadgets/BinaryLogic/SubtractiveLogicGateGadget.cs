using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class SubtractiveLogicGateGadget : FunctionalGadget<SubtractiveLogicGateGadget.SubtractiveLogicGateGadgetInput>
{
    private bool _shouldTick;

    protected SubtractiveLogicGateGadget(
        string gadgetName,
        GadgetState state0,
        GadgetState state1,
        bool startActive,
        int expectedNumberOfInputs)
        : base(gadgetName, state0, state1, startActive, expectedNumberOfInputs)
    {
    }

    protected sealed override void OnTick()
    {
        if (!_shouldTick)
            return;

        _shouldTick = false;

        var isActive = EvaluateInputs();
        SetNextState(isActive ? 1 : 0);
    }

    protected sealed override void OnChangeStates() { }

    protected abstract bool EvaluateInputs();

    public sealed class SubtractiveLogicGateGadgetInput : GadgetInput
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
}

public sealed class NotGateGadget : SubtractiveLogicGateGadget
{
    private readonly SubtractiveLogicGateGadgetInput _input;

    public NotGateGadget(
        string gadgetName,
        GadgetState state0,
        GadgetState state1,
        bool startActive,
        string inputName)
        : base(gadgetName, state0, state1, startActive, 1)
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
        string gadgetName,
        GadgetState state0,
        GadgetState state1,
        bool startActive,
        string input1Name,
        string input2Name)
        : base(gadgetName, state0, state1, startActive, 2)
    {
        _input1 = new SubtractiveLogicGateGadgetInput(input1Name, this);
        _input2 = new SubtractiveLogicGateGadgetInput(input2Name, this);
        RegisterInput(_input1);
        RegisterInput(_input2);
    }

    protected override bool EvaluateInputs()
    {
        return _input1.Signal ^ _input2.Signal;
    }
}