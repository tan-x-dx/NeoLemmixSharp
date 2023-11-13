namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class HatchGadgetType : GadgetSubType
{
    public static HatchGadgetType Instance { get; } = new ();

    private HatchGadgetType()
    {
    }

    public override int Id => LevelConstants.HatchGadgetTypeId;
    public override string GadgetTypeName => "hatch";
}