namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class LogicGateGadgetType : GadgetSubType
{
    public static readonly LogicGateGadgetType Instance = new();

    private LogicGateGadgetType()
    {
    }

    public override int Id => LevelConstants.LogicGateGadgetTypeId;
    public override string GadgetTypeName => "logic gate";
}