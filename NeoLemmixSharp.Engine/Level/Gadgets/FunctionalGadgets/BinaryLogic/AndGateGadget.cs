using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public sealed class AndGateGadget : LogicGateGadget
{
    private readonly LogicGateGadgetInput[] _inputs;

    public AndGateGadget(
        int id,
        Orientation orientation,
        ReadOnlySpan<string> inputNames)
        : base(id, orientation)
    {
        if (inputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        _inputs = new LogicGateGadgetInput[inputNames.Length];

        for (var i = 0; i < inputNames.Length; i++)
        {
            var inputName = inputNames[i];
            var input = new LogicGateGadgetInput(inputName, this);
            _inputs[i] = input;
            RegisterInput(input);
        }
    }

    public override void EvaluateInputs()
    {
        var outputSignal = true;
        for (var i = 0; i < _inputs.Length; i++)
        {
            var input = _inputs[i];
            outputSignal &= input.Signal;
        }
        Output.SetSignal(outputSignal);
    }
}
