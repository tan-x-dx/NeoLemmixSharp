using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class AdditiveLogicGateGadget : GadgetBase,
    IBitBufferCreator<ArrayBitBuffer, AdditiveLogicGateGadget.AdditiveGateGadgetLinkInput>
{
    private readonly BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetLinkInput> _set;
    private readonly int _numberOfInputs;

    protected AdditiveLogicGateGadget(
        AdditiveLogicGateGadgetState[] states,
        int initialStateIndex,
        ReadOnlySpan<GadgetTriggerName> inputNames)
    // : base(states, initialStateIndex, inputNames.Length)
    {
        _numberOfInputs = inputNames.Length;
        _set = new BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetLinkInput>(this);

        for (var i = 0; i < inputNames.Length; i++)
        {
            // var input = new AdditiveGateGadgetLinkInput(this);
            // RegisterInput(input);
        }
    }

    public sealed override void Tick() { }

    private void ReactToSignal(AdditiveGateGadgetLinkInput input, bool signal)
    {
        var hasChanged = signal
            ? _set.Add(input)
            : _set.Remove(input);
        if (!hasChanged)
            return;

        var isActive = EvaluateInputCount(_set.Count, _numberOfInputs);
        //SetNextState(isActive ? 1 : 0);
        // ChangeStates();
    }

    public sealed override AdditiveLogicGateGadgetState CurrentState => throw new NotImplementedException();

    public sealed override void SetNextState(int stateIndex)
    {
        throw new NotImplementedException();
    }

    protected abstract bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs);

    public sealed class AdditiveGateGadgetLinkInput : GadgetTrigger
    {
        private readonly AdditiveLogicGateGadget _gadget;

        public AdditiveGateGadgetLinkInput(
            AdditiveLogicGateGadget gadget)
            : base([])
        {
            _gadget = gadget;
        }

        public override void Tick()
        {
        }

        // public override void ReactToSignal(bool signal) => _gadget.ReactToSignal(this, signal);
    }

    int IPerfectHasher<AdditiveGateGadgetLinkInput>.NumberOfItems => _numberOfInputs;
    int IPerfectHasher<AdditiveGateGadgetLinkInput>.Hash(AdditiveGateGadgetLinkInput item) => item.Id;
    AdditiveGateGadgetLinkInput IPerfectHasher<AdditiveGateGadgetLinkInput>.UnHash(int index) => throw new NotSupportedException("Why are you doing this? Stop it.");
    void IBitBufferCreator<ArrayBitBuffer, AdditiveGateGadgetLinkInput>.CreateBitBuffer(out ArrayBitBuffer buffer) => buffer = new(_numberOfInputs);
}

public sealed class AndGateGadget : AdditiveLogicGateGadget
{
    public AndGateGadget(
        AdditiveLogicGateGadgetState[] states,
        int initialStateIndex,
        ReadOnlySpan<GadgetTriggerName> inputNames)
        : base(states, initialStateIndex, inputNames)
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
        AdditiveLogicGateGadgetState[] states,
        int initialStateIndex,
        ReadOnlySpan<GadgetTriggerName> inputNames)
        : base(states, initialStateIndex, inputNames)
    {
    }

    protected override bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs)
    {
        return numberOfTrueInputs > 0;
    }
}

public sealed class AdditiveLogicGateGadgetState : GadgetState
{
    public AdditiveLogicGateGadgetState(GadgetTrigger[] gadgetTriggers) : base(gadgetTriggers)
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
