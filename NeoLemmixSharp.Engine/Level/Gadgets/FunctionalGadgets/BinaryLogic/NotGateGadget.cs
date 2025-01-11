using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public sealed class NotGateGadget : LogicGateGadget
{
    private readonly LogicGateGadgetInput _input;
    private bool _shouldTick;

    public NotGateGadget(
        int id,
        Orientation orientation,
        string inputName) : base(id, orientation)
    {
        _input = new LogicGateGadgetInput(inputName, this);
        RegisterInput(_input);
    }

    public override void Tick()
    {
        if (!_shouldTick)
            return;

        _shouldTick = false;
        Output.SetSignal(!_input.Signal);
    }

    public override void EvaluateInputs()
    {
        _shouldTick = true;
    }
}
