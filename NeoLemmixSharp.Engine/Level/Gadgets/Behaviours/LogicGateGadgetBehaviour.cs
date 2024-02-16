namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class LogicGateGadgetBehaviour : GadgetBehaviour
{
    public static readonly LogicGateGadgetBehaviour Instance = new();

    private LogicGateGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.LogicGateGadgetBehaviourId;
    public override string GadgetBehaviourName => "logic gate";
}