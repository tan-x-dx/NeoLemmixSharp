using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

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

    public sealed override void Tick() { }

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
        private readonly OutputSignalBehaviour _signalBehaviour;
        private readonly GadgetBehaviour _signalBehaviourCast;

        public AdditiveLogicGateGadget Gadget { get; set; } = null!;

        public AdditiveLogicGateGadgetLinkInput(OutputSignalBehaviour signalBehaviour)
            : base(GadgetTriggerType.GadgetLinkTrigger)
        {
            _signalBehaviour = signalBehaviour;
            _signalBehaviourCast = signalBehaviour;
        }

        public override void Tick()
        {
            //_gadget.ReactToSignal(this, signal);
        }

        public override ReadOnlySpan<GadgetBehaviour> Behaviours => new(in _signalBehaviourCast);

        public void ReactToSignal(bool signal)
        {
            throw new NotImplementedException();
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
