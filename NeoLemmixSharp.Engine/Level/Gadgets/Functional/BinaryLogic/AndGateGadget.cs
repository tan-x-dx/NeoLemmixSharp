﻿using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class AndGateGadget : BinaryLogicGateGadget
{
    public AndGateGadget(
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
        var outputSignal = InputA.Signal && InputB.Signal;

        Output.SetSignal(outputSignal);
    }
}