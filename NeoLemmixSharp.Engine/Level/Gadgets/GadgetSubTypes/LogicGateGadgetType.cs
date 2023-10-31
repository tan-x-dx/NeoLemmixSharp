﻿namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class LogicGateGadgetType : GadgetSubType
{
    public static LogicGateGadgetType Instance { get; } = new();

    private LogicGateGadgetType()
    {
    }

    public override int Id => LevelConstants.LogicGateGadgetTypeId;
    public override string GadgetTypeName => "logic gate";
}