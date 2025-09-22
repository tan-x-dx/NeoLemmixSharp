using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class SubtractiveLogicGateGadget : GadgetBase, IGadgetLinkTrigger
{
    private readonly SubtractiveLogicGateGadgetState _offState;
    private readonly SubtractiveLogicGateGadgetState _onState;
    private SubtractiveLogicGateGadgetState _currentState;
    private readonly OutputSignalBehaviour _outputSignal;

    private bool _shutOff;

    public sealed override SubtractiveLogicGateGadgetState CurrentState => _currentState;

    protected SubtractiveLogicGateGadget(
        SubtractiveLogicGateGadgetState offState,
        SubtractiveLogicGateGadgetState onState,
        OutputSignalBehaviour outputSignal)
        : base(GadgetType.LogicGate)
    {
        _offState = offState;
        _onState = onState;
        _currentState = _offState;

        _offState.SetParentGadget(this);
        _onState.SetParentGadget(this);

        _outputSignal = outputSignal;
    }

    public sealed override void Tick()
    {
        if (_shutOff)
        {
            _currentState = _offState;
            _shutOff = false;
        }
        else
        {
            _currentState = _onState;
            LevelScreen.GadgetManager.RegisterCauseAndEffectData(_outputSignal);
        }

        _currentState.Tick();
    }

    public sealed override void SetState(int stateIndex)
    {
        var state = stateIndex & 1;
        _currentState = state == 0
            ? _offState
            : _onState;
    }

    public void ReactToSignal(int payload)
    {
        InputTrigger(payload);

        _shutOff = EvaluateInputs();
    }

    protected abstract void InputTrigger(int payload);

    protected abstract bool EvaluateInputs();
}

public sealed class NotGateGadget : SubtractiveLogicGateGadget
{
    private bool _input;

    public NotGateGadget(
        SubtractiveLogicGateGadgetState offState,
        SubtractiveLogicGateGadgetState onState,
        OutputSignalBehaviour outputSignal)
        : base(offState, onState, outputSignal)
    {
    }

    protected override void InputTrigger(int payload)
    {
        _input = true;
    }

    protected override bool EvaluateInputs()
    {
        return !_input;
    }
}

public sealed class XorGateGadget : SubtractiveLogicGateGadget
{
    private bool _input0;
    private bool _input1;

    public XorGateGadget(
        SubtractiveLogicGateGadgetState offState,
        SubtractiveLogicGateGadgetState onState,
        OutputSignalBehaviour outputSignal)
        : base(offState, onState, outputSignal)
    {
    }

    protected override void InputTrigger(int payload)
    {
        var i = payload & 1;
        if (i == 0)
        {
            _input0 = true;
        }
        else
        {
            _input1 = true;
        }
    }

    protected override bool EvaluateInputs()
    {
        return _input0 ^ _input1;
    }
}

public sealed class SubtractiveLogicGateGadgetState : GadgetState
{
    public override GadgetRenderer Renderer { get; }
}
