﻿using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public abstract class BinaryLogicGateGadget : GadgetBase, ILogicGateGadget
{
    public sealed override Orientation Orientation => DownOrientation.Instance;

    public LogicGateGadgetInput InputA { get; }
    public LogicGateGadgetInput InputB { get; }
    public GadgetOutput Output { get; } = new();
    public sealed override GadgetBehaviour GadgetBehaviour => LogicGateGadgetBehaviour.Instance;

    protected BinaryLogicGateGadget(
        int id,
        RectangularHitBoxRegion gadgetBounds,
        IControlledAnimationGadgetRenderer? renderer,
        LogicGateGadgetInput inputA,
        LogicGateGadgetInput inputB)
        : base(id, gadgetBounds, renderer)
    {
        Debug.Assert(!string.Equals(inputA.InputName, inputB.InputName));

        InputA = inputA;
        InputB = inputB;

        InputA.SetLogicGate(this);
        InputB.SetLogicGate(this);
    }

    public IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, InputA.InputName))
            return InputA;

        if (string.Equals(inputName, InputB.InputName))
            return InputB;

        return null;
    }

    public abstract void EvaluateInputs();
}