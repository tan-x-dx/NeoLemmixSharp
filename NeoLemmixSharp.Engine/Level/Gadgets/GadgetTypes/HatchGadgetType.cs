namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class HatchGadgetType : GadgetType
{
    public static HatchGadgetType Instance { get; } = new ();

    private HatchGadgetType()
    {
    }

    public override int Id => Global.HatchGadgetTypeId;
    public override string GadgetTypeName => "hatch";
}