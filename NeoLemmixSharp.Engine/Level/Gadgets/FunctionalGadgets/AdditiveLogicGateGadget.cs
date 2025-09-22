using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class AdditiveLogicGateGadget : GadgetBase,
    IGadgetLinkTrigger,
    IBitBufferCreator<ArrayBitBuffer, int>
{
    private readonly AdditiveLogicGateGadgetState _offState;
    private readonly AdditiveLogicGateGadgetState _onState;
    private AdditiveLogicGateGadgetState _currentState;
    private readonly OutputSignalBehaviour _outputSignal;
    private readonly BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, int> _set;

    private readonly int _numberOfInputs;

    protected AdditiveLogicGateGadget(
        AdditiveLogicGateGadgetState offState,
        AdditiveLogicGateGadgetState onState,
        OutputSignalBehaviour outputSignal,
        int numberOfInputs)
        : base(GadgetType.LogicGate)
    {
        _offState = offState;
        _onState = onState;
        _currentState = _offState;
        _outputSignal = outputSignal;

        _set = new BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, int>(this);
        _numberOfInputs = numberOfInputs;

        _offState.SetParentGadget(this);
        _onState.SetParentGadget(this);
    }

    public sealed override void Tick()
    {
        _set.Clear();
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
        if (!_set.Add(payload))
            return;

        var isActive = EvaluateInputCount(_set.Count, _numberOfInputs);
        if (isActive)
        {
            LevelScreen.GadgetManager.RegisterCauseAndEffectData(_outputSignal);
            _currentState = _onState;
        }
        else
        {
            _currentState = _offState;
        }
    }

    protected abstract bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs);

    public sealed override AdditiveLogicGateGadgetState CurrentState => _currentState;

    int IPerfectHasher<int>.NumberOfItems => _numberOfInputs;
    int IPerfectHasher<int>.Hash(int item) => item;
    int IPerfectHasher<int>.UnHash(int index) => index;
    void IBitBufferCreator<ArrayBitBuffer, int>.CreateBitBuffer(out ArrayBitBuffer buffer) => buffer = new(_numberOfInputs);
}

public sealed class AndGateGadget : AdditiveLogicGateGadget
{
    public AndGateGadget(
        AdditiveLogicGateGadgetState offState,
        AdditiveLogicGateGadgetState onState,
        OutputSignalBehaviour outputSignal,
        int numberOfInputs)
        : base(offState, onState, outputSignal, numberOfInputs)
    {
    }

    protected override bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs)
    {
        return numberOfTrueInputs == numberOfInputs;
    }
}

public sealed class OrGateGadget : AdditiveLogicGateGadget
{
    public OrGateGadget(
        AdditiveLogicGateGadgetState offState,
        AdditiveLogicGateGadgetState onState,
        OutputSignalBehaviour outputSignal,
        int numberOfInputs)
        : base(offState, onState, outputSignal, numberOfInputs)
    {
    }

    protected override bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs)
    {
        return numberOfTrueInputs > 0;
    }
}

public sealed class AdditiveLogicGateGadgetState : GadgetState
{
    public override GadgetRenderer Renderer { get; }
}
