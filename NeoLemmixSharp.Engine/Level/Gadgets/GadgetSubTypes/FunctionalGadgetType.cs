namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class FunctionalGadgetType : GadgetSubType
{
    public static FunctionalGadgetType Instance { get; } = new();

    private FunctionalGadgetType()
    {
    }

    public override int Id => Global.FunctionalGadgetTypeId;
    public override string GadgetTypeName => "functional";
}