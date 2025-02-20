using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class AdditiveLogicGateGadget : GadgetBase,
    IPerfectHasher<AdditiveLogicGateGadget.AdditiveGateGadgetInput>,
    IBitBufferCreator<ArrayBitBuffer>,
    ILogicGateGadget,
    ISimpleRenderGadget
{
    private SimpleGadgetRenderer _renderer;
    private readonly BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetInput> _set;
    private readonly int _numberOfInputs;

    public LogicGateType Type { get; }

    public GadgetOutput Output { get; } = new();

    public sealed override SimpleGadgetRenderer Renderer => _renderer;

    protected AdditiveLogicGateGadget(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        LogicGateType type,
        ReadOnlySpan<string> inputNames)
        : base(id, orientation, gadgetBounds, inputNames.Length)
    {
        if (inputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        _numberOfInputs = inputNames.Length;
        _set = new BitArraySet<AdditiveLogicGateGadget, ArrayBitBuffer, AdditiveGateGadgetInput>(this, false);

        Type = type;

        for (var i = 0; i < inputNames.Length; i++)
        {
            var input = new AdditiveGateGadgetInput(i, inputNames[i], this);
            RegisterInput(input);
        }
    }

    public sealed override void Tick() { }

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

        public override void ReactToSignal(bool signal)
        {
            if (signal)
            {
                _gadget._set.Add(this);
            }
            else
            {
                _gadget._set.Remove(this);
            }
            _gadget.Output.SetSignal(_gadget.EvaluateInputCount(_gadget._set.Count, _gadget._numberOfInputs));
        }
    }

    SimpleGadgetRenderer ISimpleRenderGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }

    int IPerfectHasher<AdditiveGateGadgetInput>.NumberOfItems => _numberOfInputs;
    int IPerfectHasher<AdditiveGateGadgetInput>.Hash(AdditiveGateGadgetInput item) => item.Id;
    AdditiveGateGadgetInput IPerfectHasher<AdditiveGateGadgetInput>.UnHash(int index) => throw new NotSupportedException("Why are you doing this? Stop it.");
    void IBitBufferCreator<ArrayBitBuffer>.CreateBitBuffer(out ArrayBitBuffer buffer) => buffer = new(_numberOfInputs);
}

public sealed class AndGateGadget : AdditiveLogicGateGadget
{
    public AndGateGadget(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        ReadOnlySpan<string> inputNames)
        : base(id, orientation, gadgetBounds, LogicGateType.AndGate, inputNames)
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
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        ReadOnlySpan<string> inputNames)
        : base(id, orientation, gadgetBounds, LogicGateType.OrGate, inputNames)
    {
    }

    protected override bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs)
    {
        return numberOfTrueInputs > 0;
    }
}