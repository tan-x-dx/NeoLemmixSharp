namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class HatchGadgetType : GadgetSubType
{
    public static readonly HatchGadgetType Instance = new();

    private HatchGadgetType()
    {
    }

    public override int Id => LevelConstants.HatchGadgetTypeId;
    public override string GadgetTypeName => "hatch";
}