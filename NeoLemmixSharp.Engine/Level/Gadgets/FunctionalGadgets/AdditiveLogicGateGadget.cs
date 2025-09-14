using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class AdditiveLogicGateGadget : GadgetBase,
    IBitBufferCreator<ArrayBitBuffer, AdditiveLogicGateGadget.AdditiveLogicGateGadgetLinkInput>
{
    private readonly AdditiveLogicGateGadgetState _offState;
    private readonly AdditiveLogicGateGadgetState _onState;

    private AdditiveLogicGateGadgetState _currentState;

    private readonly AdditiveLogicGateGadgetLinkInput[] _inputs;
    private readonly BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveLogicGateGadgetLinkInput> _set;

    protected AdditiveLogicGateGadget(
        AdditiveLogicGateGadgetState offState,
        AdditiveLogicGateGadgetState onState,
        AdditiveLogicGateGadgetLinkInput[] inputs)
        : base(GadgetType.LogicGate)
    {
        _offState = offState;
        _onState = onState;
        _currentState = _offState;

        _inputs = inputs;
        _set = new BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveLogicGateGadgetLinkInput>(this);

        foreach (var input in _inputs)
        {
            input.Gadget = this;
        }
    }

    public sealed override void Tick()
    {
        _currentState.Tick(this);
    }

    private void ReactToSignal(AdditiveLogicGateGadgetLinkInput input, bool signal)
    {
        var hasChanged = signal
            ? _set.Add(input)
            : _set.Remove(input);
        if (!hasChanged)
            return;

        var isActive = EvaluateInputCount(_set.Count, _inputs.Length);
        _currentState = isActive ? _onState : _offState;
    }

    protected abstract bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs);

    public sealed override AdditiveLogicGateGadgetState CurrentState => _currentState;

    public sealed class AdditiveLogicGateGadgetLinkInput : GadgetTrigger, IGadgetLinkTrigger
    {
        public OutputSignalBehaviour? InputSignalBehaviour { get; set; }

        public AdditiveLogicGateGadget Gadget { get; set; } = null!;

        public AdditiveLogicGateGadgetLinkInput()
            : base(GadgetTriggerType.GadgetLinkTrigger)
        {
        }

        public override void DetectTrigger(GadgetBase _)
        {
            if (Evaluation != TriggerEvaluation.Indeterminate)
            {
                LevelScreen.GadgetManager.FlagGadgetForReEvaluation(Gadget);
            }
        }

        public void ReactToSignal(bool signal)
        {
            DetermineTrigger(signal);
            MarkAsEvaluated();
            LevelScreen.GadgetManager.FlagGadgetForReEvaluation(Gadget);
        }
    }

    int IPerfectHasher<AdditiveLogicGateGadgetLinkInput>.NumberOfItems => _inputs.Length;
    int IPerfectHasher<AdditiveLogicGateGadgetLinkInput>.Hash(AdditiveLogicGateGadgetLinkInput item) => item.Id;
    AdditiveLogicGateGadgetLinkInput IPerfectHasher<AdditiveLogicGateGadgetLinkInput>.UnHash(int index) => throw new NotSupportedException("Why are you doing this? Stop it.");
    void IBitBufferCreator<ArrayBitBuffer, AdditiveLogicGateGadgetLinkInput>.CreateBitBuffer(out ArrayBitBuffer buffer) => buffer = new(_inputs.Length);
}

public sealed class AndGateGadget : AdditiveLogicGateGadget
{
    public AndGateGadget(
        AdditiveLogicGateGadgetState offState,
        AdditiveLogicGateGadgetState onState,
        AdditiveLogicGateGadgetLinkInput[] inputs)
        : base(offState, onState, inputs)
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
        AdditiveLogicGateGadgetLinkInput[] inputs)
        : base(offState, onState, inputs)
    {
    }

    protected override bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs)
    {
        return numberOfTrueInputs > 0;
    }
}

public sealed class AdditiveLogicGateGadgetState : GadgetState
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
