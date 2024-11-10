using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class LogicGateGadgetBehaviour : GadgetBehaviour
{
    public static readonly LogicGateGadgetBehaviour Instance = new();

    private LogicGateGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.LogicGateGadgetBehaviourId;
    public override string GadgetBehaviourName => "logic gate";
}