﻿using NeoLemmixSharp.Common.Util.LevelRegion;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class XorGateGadget : BinaryLogicGateGadget
{
    private bool _shouldTick;

    public override GadgetType Type => GadgetType.XorGate;

    public XorGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        BinaryLogicGateGadgetInput inputA,
        BinaryLogicGateGadgetInput inputB)
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