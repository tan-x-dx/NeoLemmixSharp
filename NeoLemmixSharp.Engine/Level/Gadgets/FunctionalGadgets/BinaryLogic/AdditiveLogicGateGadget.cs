﻿using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class AdditiveLogicGateGadget : FunctionalGadget<AdditiveLogicGateGadget.AdditiveGateGadgetInput>,
    IPerfectHasher<AdditiveLogicGateGadget.AdditiveGateGadgetInput>,
    IBitBufferCreator<ArrayBitBuffer>
{
    private readonly BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetInput> _set;
    private readonly int _numberOfInputs;

    protected AdditiveLogicGateGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        ReadOnlySpan<GadgetInputName> inputNames)
        : base(gadgetName, states, initialStateIndex, inputNames.Length)
    {
        _numberOfInputs = inputNames.Length;
        _set = new BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetInput>(this);

        for (var i = 0; i < inputNames.Length; i++)
        {
            var input = new AdditiveGateGadgetInput(i, inputNames[i], this);
            RegisterInput(input);
        }
    }

    protected sealed override void OnTick() { }
    protected sealed override void OnChangeStates() { }

    private void ReactToSignal(AdditiveGateGadgetInput input, bool signal)
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

    public sealed class AdditiveGateGadgetInput : GadgetInput
    {
        public readonly int Id;
        private readonly AdditiveLogicGateGadget _gadget;

        public AdditiveGateGadgetInput(
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

    int IPerfectHasher<AdditiveGateGadgetInput>.NumberOfItems => _numberOfInputs;
    int IPerfectHasher<AdditiveGateGadgetInput>.Hash(AdditiveGateGadgetInput item) => item.Id;
    AdditiveGateGadgetInput IPerfectHasher<AdditiveGateGadgetInput>.UnHash(int index) => throw new NotSupportedException("Why are you doing this? Stop it.");
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
