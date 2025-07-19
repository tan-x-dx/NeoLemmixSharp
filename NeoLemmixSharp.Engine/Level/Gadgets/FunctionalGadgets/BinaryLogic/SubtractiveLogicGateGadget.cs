using NeoLemmixSharp.Engine.Level.Gadgets.Triggers;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class SubtractiveLogicGateGadget : FunctionalGadget<SubtractiveLogicGateGadget.SubtractiveLogicGateGadgetLinkInput>
{
    private bool _shouldTick;

    protected SubtractiveLogicGateGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        int expectedNumberOfInputs)
        : base(gadgetName, states, initialStateIndex, expectedNumberOfInputs)
    {
    }

    public sealed override void Tick()
    {
        if (!_shouldTick)
            return;

        _shouldTick = false;

        var isActive = EvaluateInputs();
        //   SetNextState(isActive ? 1 : 0);
    }

    public sealed override GadgetState CurrentState => throw new NotImplementedException();

    public sealed override void SetNextState(int stateIndex)
    {
        throw new NotImplementedException();
    }

    protected abstract bool EvaluateInputs();

    public sealed class SubtractiveLogicGateGadgetLinkInput : GadgetLinkTrigger
    {
        private readonly SubtractiveLogicGateGadget _gadget;

        public bool Signal { get; private set; }

        public SubtractiveLogicGateGadgetLinkInput(GadgetTriggerName inputName, SubtractiveLogicGateGadget gadget)
            : base(inputName, [])
        {
            _gadget = gadget;
        }

        public override void OnNewTick()
        {
            ResetGeneralBehaviours();
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
    private readonly SubtractiveLogicGateGadgetLinkInput _input;

    public NotGateGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        GadgetTriggerName inputName)
        : base(gadgetName, states, initialStateIndex, 1)
    {
        _input = new SubtractiveLogicGateGadgetLinkInput(inputName, this);
        RegisterInput(_input);
    }

    protected override bool EvaluateInputs()
    {
        return !_input.Signal;
    }
}

public sealed class XorGateGadget : SubtractiveLogicGateGadget
{
    private readonly SubtractiveLogicGateGadgetLinkInput _input1;
    private readonly SubtractiveLogicGateGadgetLinkInput _input2;

    public XorGateGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        GadgetTriggerName input1Name,
        GadgetTriggerName input2Name)
        : base(gadgetName, states, initialStateIndex, 2)
    {
        _input1 = new SubtractiveLogicGateGadgetLinkInput(input1Name, this);
        _input2 = new SubtractiveLogicGateGadgetLinkInput(input2Name, this);
        RegisterInput(_input1);
        RegisterInput(_input2);
    }

    protected override bool EvaluateInputs()
    {
        return _input1.Signal ^ _input2.Signal;
    }
}