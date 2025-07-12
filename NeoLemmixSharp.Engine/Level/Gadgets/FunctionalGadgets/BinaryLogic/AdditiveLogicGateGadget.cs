using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class AdditiveLogicGateGadget : FunctionalGadget<AdditiveLogicGateGadget.AdditiveGateGadgetLinkInput>,
    IPerfectHasher<AdditiveLogicGateGadget.AdditiveGateGadgetLinkInput>,
    IBitBufferCreator<ArrayBitBuffer>
{
    private readonly BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetLinkInput> _set;
    private readonly int _numberOfInputs;

    protected AdditiveLogicGateGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        ReadOnlySpan<GadgetInputName> inputNames)
        : base(gadgetName, states, initialStateIndex, inputNames.Length)
    {
        _numberOfInputs = inputNames.Length;
        _set = new BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetLinkInput>(this);

        for (var i = 0; i < inputNames.Length; i++)
        {
            var input = new AdditiveGateGadgetLinkInput(i, inputNames[i], this);
            RegisterInput(input);
        }
    }

    protected sealed override void OnTick() { }
    protected sealed override void OnChangeStates() { }

    private void ReactToSignal(AdditiveGateGadgetLinkInput input, bool signal)
    {
        var hasChanged = signal
            ? _set.Add(input)
            : _set.Remove(input);
        if (!hasChanged)
            return;

        var isActive = EvaluateInputCount(_set.Count, _numberOfInputs);
        SetNextState(isActive ? 1 : 0);
        ChangeStates();
    }

    protected abstract bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs);

    public sealed class AdditiveGateGadgetLinkInput : GadgetLinkInput
    {
        public readonly int Id;
        private readonly AdditiveLogicGateGadget _gadget;

        public AdditiveGateGadgetLinkInput(
            int id,
            GadgetInputName inputName,
            AdditiveLogicGateGadget gadget)
            : base(inputName)
        {
            Id = id;
            _gadget = gadget;
        }

        public override void ReactToSignal(bool signal) => _gadget.ReactToSignal(this, signal);
    }

    int IPerfectHasher<AdditiveGateGadgetLinkInput>.NumberOfItems => _numberOfInputs;
    int IPerfectHasher<AdditiveGateGadgetLinkInput>.Hash(AdditiveGateGadgetLinkInput item) => item.Id;
    AdditiveGateGadgetLinkInput IPerfectHasher<AdditiveGateGadgetLinkInput>.UnHash(int index) => throw new NotSupportedException("Why are you doing this? Stop it.");
    void IBitBufferCreator<ArrayBitBuffer>.CreateBitBuffer(out ArrayBitBuffer buffer) => buffer = new(_numberOfInputs);
}

public sealed class AndGateGadget : AdditiveLogicGateGadget
{
    public AndGateGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        ReadOnlySpan<GadgetInputName> inputNames)
        : base(gadgetName, states, initialStateIndex, inputNames)
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
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        ReadOnlySpan<GadgetInputName> inputNames)
        : base(gadgetName, states, initialStateIndex, inputNames)
    {
    }

    protected override bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs)
    {
        return numberOfTrueInputs > 0;
    }
}
