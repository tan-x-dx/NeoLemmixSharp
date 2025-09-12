using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class SubtractiveLogicGateGadget : GadgetBase
{
    private readonly SubtractiveLogicGateGadgetState _offState;
    private readonly SubtractiveLogicGateGadgetState _onState;

    private SubtractiveLogicGateGadgetState _currentState;
    private bool _shouldTick;

    protected SubtractiveLogicGateGadget(
        SubtractiveLogicGateGadgetState offState,
        SubtractiveLogicGateGadgetState onState)
        : base(Common.Enums.GadgetType.LogicGate)
    {
        _offState = offState;
        _onState = onState;

        _currentState = _offState;
    }

    public sealed override void Tick()
    {
        if (!_shouldTick)
            return;

        _shouldTick = false;

        var isActive = EvaluateInputs();
        _currentState = isActive ? _onState : _offState;
    }

    protected abstract bool EvaluateInputs();

    public sealed override SubtractiveLogicGateGadgetState CurrentState => _currentState;

    public sealed class SubtractiveLogicGateGadgetLinkInput : GadgetTrigger, IGadgetLinkTrigger
    {
        public OutputSignalBehaviour? InputSignalBehaviour { get; set; }

        public SubtractiveLogicGateGadget Gadget { get; set; }

        public bool Signal { get; private set; }

        public SubtractiveLogicGateGadgetLinkInput( )
            : base(GadgetTriggerType.GadgetLinkTrigger)
        {
        }

        public override void DetectTrigger(GadgetBase parentGadget)
        {
        }

        public void ReactToSignal(bool signal)
        {
            DetermineTrigger(signal);
            LevelScreen.CauseAndEffectManager.MarkTriggerAsEvaluated(this);
            LevelScreen.GadgetManager.FlagGadgetForReEvaluation(Gadget);
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
        SubtractiveLogicGateGadgetState offState,
        SubtractiveLogicGateGadgetState onState,
        SubtractiveLogicGateGadgetLinkInput input)
        : base(offState, onState)
    {
        _input = input;
        _input.Gadget = this;
    }

    protected override bool EvaluateInputs()
    {
        return !_input.Signal;
    }
}

public sealed class XorGateGadget : SubtractiveLogicGateGadget
{
    private readonly SubtractiveLogicGateGadgetLinkInput _input0;
    private readonly SubtractiveLogicGateGadgetLinkInput _input1;

    public XorGateGadget(
        SubtractiveLogicGateGadgetState offState,
        SubtractiveLogicGateGadgetState onState,
        SubtractiveLogicGateGadgetLinkInput input0,
        SubtractiveLogicGateGadgetLinkInput input1)
        : base(offState, onState)
    {
        _input0 = input0;
        _input1 = input1;
        _input0.Gadget = this;
        _input1.Gadget = this;
    }

    protected override bool EvaluateInputs()
    {
        return _input0.Signal ^ _input1.Signal;
    }
}

public sealed class SubtractiveLogicGateGadgetState : GadgetState
{
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
