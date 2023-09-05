using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class AndGateGadget : BinaryLogicGateGadget
{
    public override GadgetType Type => GadgetType.AndGate;

    public AndGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        BinaryLogicGateGadgetInput inputA,
        BinaryLogicGateGadgetInput inputB)
        : base(id, gadgetBounds, inputA, inputB)
    {
    }

    public override void Tick()
    {
    }

    public override void EvaluateInputs()
    {
        var outputSignal = InputA.Signal && InputB.Signal;

        Output.SetSignal(outputSignal);
    }
}