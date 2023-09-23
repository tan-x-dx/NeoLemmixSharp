namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class FunctionalGadgetType : GadgetType
{
    public static FunctionalGadgetType Instance { get; } = new();

    private FunctionalGadgetType()
    {
    }

    public override int Id => Global.FunctionalGadgetTypeId;
    public override string GadgetTypeName => "functional";
}