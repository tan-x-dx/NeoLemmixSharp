using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class OrGateGadget : BinaryLogicGateGadget
{
    public override GadgetType Type => GadgetType.OrGate;

    public OrGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        LogicGateGadgetInput inputA,
        LogicGateGadgetInput inputB)
        : base(id, gadgetBounds, inputA, inputB)
    {
    }

    public override void Tick()
    {
    }

    public override void EvaluateInputs()
    {
        var outputSignal = InputA.Signal || InputB.Signal;

        Output.SetSignal(outputSignal);
    }
}