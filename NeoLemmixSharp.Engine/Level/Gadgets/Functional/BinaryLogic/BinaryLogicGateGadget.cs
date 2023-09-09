﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public abstract class BinaryLogicGateGadget : GadgetBase, ILogicGateGadget
{
    public sealed override Orientation Orientation => DownOrientation.Instance;

    public LogicGateGadgetInput InputA { get; }
    public LogicGateGadgetInput InputB { get; }
    public GadgetOutput Output { get; } = new();

    protected BinaryLogicGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        LogicGateGadgetInput inputA,
        LogicGateGadgetInput inputB)
        : base(id, gadgetBounds)
    {
        Debug.Assert(!string.Equals(inputA.InputName, inputB.InputName));

        InputA = inputA;
        InputB = inputB;

        InputA.SetLogicGate(this);
        InputB.SetLogicGate(this);
    }

    public sealed override IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, InputA.InputName))
            return InputA;

        if (string.Equals(inputName, InputB.InputName))
            return InputB;

        return null;
    }

    public sealed override bool CaresAboutLemmingInteraction => false;
    public sealed override bool MatchesLemming(Lemming lemming) => false;
    public sealed override void OnLemmingMatch(Lemming lemming)
    {
    }

    public sealed override bool MatchesPosition(LevelPosition levelPosition) => false;

    public abstract void EvaluateInputs();
}