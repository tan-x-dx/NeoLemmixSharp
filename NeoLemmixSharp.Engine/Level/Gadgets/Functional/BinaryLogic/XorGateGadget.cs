using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class XorGateGadget : BinaryLogicGateGadget
{
    private bool _shouldTick;

    public XorGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        IGadgetRenderer? renderer,
        LogicGateGadgetInput inputA,
        LogicGateGadgetInput inputB)
        : base(id, gadgetBounds, renderer, inputA, inputB)
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