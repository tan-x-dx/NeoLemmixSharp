using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public abstract class AdditiveLogicGateGadget : GadgetBase, ISimpleRenderGadget
{
    private SimpleGadgetRenderer _renderer;
    private readonly uint[] _inputBits;
    private readonly int _numberOfInputs;
    private int _popCount;

    public sealed override SimpleGadgetRenderer Renderer => _renderer;

    public GadgetOutput Output { get; } = new();

    public AdditiveLogicGateGadget(
        int id,
        Orientation orientation,
        ReadOnlySpan<string> inputNames)
        : base(id, orientation)
    {
        if (inputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        _numberOfInputs = inputNames.Length;
        _inputBits = BitArrayHelpers.CreateBitArray(_numberOfInputs, false);

        for (var i = 0; i < inputNames.Length; i++)
        {
            var input = new AdditiveGateGadgetInput(i, inputNames[i], this);
            RegisterInput(input);
        }
    }

    public sealed override void Tick() { }

    protected abstract bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs);

    private sealed class AdditiveGateGadgetInput : IGadgetInput
    {
        private readonly int _id;
        private readonly AdditiveLogicGateGadget _gadget;
        public string InputName { get; }

        public AdditiveGateGadgetInput(
            int id,
            string inputName,
            AdditiveLogicGateGadget gadget)
        {
            _id = id;
            InputName = inputName;
            _gadget = gadget;
        }

        public void OnRegistered()
        {
        }

        public void ReactToSignal(bool signal)
        {
            if (signal)
            {
                BitArrayHelpers.SetBit(_gadget._inputBits, _id, ref _gadget._popCount);
            }
            else
            {
                BitArrayHelpers.ClearBit(_gadget._inputBits, _id, ref _gadget._popCount);
            }
            _gadget.Output.SetSignal(_gadget.EvaluateInputCount(_gadget._popCount, _gadget._numberOfInputs));
        }
    }

    SimpleGadgetRenderer ISimpleRenderGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
}

public sealed class AndGateGadget : AdditiveLogicGateGadget
{
    public AndGateGadget(
        int id,
        Orientation orientation,
        ReadOnlySpan<string> inputNames)
        : base(id, orientation, inputNames)
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
        ReadOnlySpan<string> inputNames)
        : base(id, orientation, inputNames)
    {
    }

    protected override bool EvaluateInputCount(int numberOfTrueInputs, int numberOfInputs)
    {
        return numberOfTrueInputs > 0;
    }
}