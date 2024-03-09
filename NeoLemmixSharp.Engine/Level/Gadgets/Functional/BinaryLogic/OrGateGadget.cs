using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class OrGateGadget : BinaryLogicGateGadget
{
    public OrGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        IViewportObjectRenderer? renderer,
        LogicGateGadgetInput inputA,
        LogicGateGadgetInput inputB)
        : base(id, gadgetBounds, renderer, inputA, inputB)
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