﻿using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class FireGadgetBehaviour : GadgetBehaviour
{
    public static readonly FireGadgetBehaviour Instance = new();

    private FireGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.FireGadgetBehaviourId;
    public override string GadgetBehaviourName => "fire";
}