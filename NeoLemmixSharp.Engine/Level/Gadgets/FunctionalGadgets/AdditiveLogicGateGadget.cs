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

        _offState.SetParentGadget(this);
        _onState.SetParentGadget(this);

        foreach (var input in _inputs)
        {
            input.ParentGadget = this;
        }
    }

    public sealed override void Tick()
    {
        _currentState.Tick();
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

        public new AdditiveLogicGateGadget ParentGadget { get; set; } = null!;

        public AdditiveLogicGateGadgetLinkInput()
            : base(GadgetTriggerType.GadgetLinkTrigger)
        {
        }

        public override void DetectTrigger()
        {
            if (InputSignalBehaviour is null)
            {
                DetermineTrigger(false);
                MarkAsEvaluated();
                return;
            }

            if (IsIndeterminate)
            {
                LevelScreen.GadgetManager.FlagGadgetForReEvaluation(ParentGadget);
            }
        }

        public void ReactToSignal()
        {
            DetermineTrigger(true);
            MarkAsEvaluated();
            LevelScreen.GadgetManager.FlagGadgetForReEvaluation(ParentGadget);
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
    public override GadgetRenderer Renderer { get; }
}
