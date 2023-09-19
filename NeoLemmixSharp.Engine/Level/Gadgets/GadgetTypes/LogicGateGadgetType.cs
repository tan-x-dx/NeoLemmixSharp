namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class LogicGateGadgetType : GadgetType
{
    public static LogicGateGadgetType Instance { get; } = new();

    private LogicGateGadgetType()
    {
    }

    public override int Id => GameConstants.LogicGateGadgetTypeId;
    public override string GadgetTypeName => "logic gate";
}