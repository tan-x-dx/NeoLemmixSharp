using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public sealed class XorGateGadget : LogicGateGadget
{
    private readonly LogicGateGadgetInput _input1;
    private readonly LogicGateGadgetInput _input2;
    private bool _shouldTick;

    public XorGateGadget(
        int id,
        Orientation orientation,
        string input1Name,
        string input2Name)
        : base(id, orientation)
    {
        _input1 = new LogicGateGadgetInput(input1Name, this);
        RegisterInput(_input1);
        _input2 = new LogicGateGadgetInput(input2Name, this);
        RegisterInput(_input2);
    }

    public override void Tick()
    {
        if (!_shouldTick)
            return;

        _shouldTick = false;

        Output.SetSignal(_input1.Signal != _input2.Signal);
    }

    public override void EvaluateInputs()
    {
        _shouldTick = true;
    }
}
