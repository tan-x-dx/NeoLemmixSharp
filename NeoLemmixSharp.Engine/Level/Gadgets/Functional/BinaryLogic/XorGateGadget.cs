using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class XorGateGadget : BinaryLogicGateGadget
{
    private bool _shouldTick;

    public XorGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        LogicGateGadgetInput inputA,
        LogicGateGadgetInput inputB)
        : base(id, gadgetBounds, inputA, inputB)
    {
    }

    public override void Tick()
    {
        if (!_shouldTick)
            return;

        _shouldTick = false;

        Output.SetSignal(InputA.Signal != InputB.Signal);
    }

    public override void EvaluateInputs()
    {
        _shouldTick = true;
    }
}