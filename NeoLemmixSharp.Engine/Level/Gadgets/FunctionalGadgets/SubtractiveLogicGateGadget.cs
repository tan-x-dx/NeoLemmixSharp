using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class SubtractiveLogicGateGadget : GadgetBase
{
    private bool _shouldTick;

    protected SubtractiveLogicGateGadget(
        GadgetState[] states,
        int initialStateIndex,
        int expectedNumberOfInputs)
    //: base(states, initialStateIndex, expectedNumberOfInputs)
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

    public sealed class SubtractiveLogicGateGadgetLinkInput : GadgetTrigger
    {
        private readonly SubtractiveLogicGateGadget _gadget;

        public bool Signal { get; private set; }

        public SubtractiveLogicGateGadgetLinkInput(SubtractiveLogicGateGadget gadget)
            : base(0)
        {
            _gadget = gadget;
        }

        public override void Tick()
        {
        }

        /* public override void ReactToSignal(bool signal)
         {
             Signal = signal;
             _gadget._shouldTick = true;
         }*/
    }
}

public sealed class NotGateGadget : SubtractiveLogicGateGadget
{
    private readonly SubtractiveLogicGateGadgetLinkInput _input;

    public NotGateGadget(
        GadgetState[] states,
        int initialStateIndex,
        GadgetTriggerName inputName)
        : base(states, initialStateIndex, 1)
    {
        // _input = new SubtractiveLogicGateGadgetLinkInput(-1, inputName, this);
        // RegisterInput(_input);
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
        GadgetState[] states,
        int initialStateIndex,
        GadgetTriggerName input1Name,
        GadgetTriggerName input2Name)
        : base(states, initialStateIndex, 2)
    {
        // _input1 = new SubtractiveLogicGateGadgetLinkInput(-1, input1Name, this);
        // _input2 = new SubtractiveLogicGateGadgetLinkInput(-1, input2Name, this);
        // RegisterInput(_input1);
        // RegisterInput(_input2);
    }

    protected override bool EvaluateInputs()
    {
        return _input1.Signal ^ _input2.Signal;
    }
}

public sealed class SubtractiveLogicGateGadgetState : GadgetState
{
    public SubtractiveLogicGateGadgetState(GadgetTrigger[] gadgetTriggers) 
    {
    }

    protected override void OnTick()
    {
        throw new NotImplementedException();
    }

    public override void OnTransitionFrom()
    {
        throw new NotImplementedException();
    }

    public override void OnTransitionTo()
    {
        throw new NotImplementedException();
    }

    public override GadgetRenderer Renderer { get; }
}
