using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class AdditiveLogicGateGadget : FunctionalGadget,
    IPerfectHasher<AdditiveLogicGateGadget.AdditiveGateGadgetInput>,
    IBitBufferCreator<ArrayBitBuffer>
{
    private readonly AnimationController _inactiveAnimationController;
    private readonly AnimationController _activeAnimationController;
    private readonly BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetInput> _set;
    private readonly int _numberOfInputs;

    public GadgetOutput Output { get; } = new();

    protected AdditiveLogicGateGadget(
        AnimationController inactiveAnimationController,
        AnimationController activeAnimationController,
        ReadOnlySpan<string> inputNames)
        : base(inputNames.Length)
    {
        _inactiveAnimationController = inactiveAnimationController;
        _activeAnimationController = activeAnimationController;
        CurrentAnimationController = inactiveAnimationController;

        _numberOfInputs = inputNames.Length;
        _set = new BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetInput>(this, false);

        for (var i = 0; i < inputNames.Length; i++)
        {
            var input = new AdditiveGateGadgetInput(i, inputNames[i], this);
            RegisterInput(input);
        }
    }

    public sealed override void Tick() => CurrentAnimationController.Tick();

    private void ReactToSignal(AdditiveGateGadgetInput input, bool signal)
    {
        var hasChanged = signal
            ? _set.Add(input)
            : _set.Remove(input);
        if (!hasChanged)
            return;

        var isActive = EvaluateInputCount(_set.Count, _numberOfInputs);
        Output.SetSignal(isActive);

        CurrentAnimationController = isActive
            ? _activeAnimationController
            : _inactiveAnimationController;
    }

    protected abstract bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs);

    private sealed class AdditiveGateGadgetInput : GadgetInput
    {
        public readonly int Id;
        private readonly AdditiveLogicGateGadget _gadget;

        public AdditiveGateGadgetInput(
            int id,
            string inputName,
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
        AnimationController inactiveAnimationController,
        AnimationController activeAnimationController,
        ReadOnlySpan<string> inputNames)
        : base(inactiveAnimationController, activeAnimationController, inputNames)
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
        AnimationController inactiveAnimationController,
        AnimationController activeAnimationController,
        ReadOnlySpan<string> inputNames)
        : base(inactiveAnimationController, activeAnimationController, inputNames)
    {
    }

    protected override bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs)
    {
        return numberOfTrueInputs > 0;
    }
}
