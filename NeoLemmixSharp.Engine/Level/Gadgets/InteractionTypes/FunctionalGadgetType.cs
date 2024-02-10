namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class FunctionalGadgetType : GadgetSubType
{
    public static readonly FunctionalGadgetType Instance = new();

    private FunctionalGadgetType()
    {
    }

    public override int Id => LevelConstants.FunctionalGadgetTypeId;
    public override string GadgetTypeName => "functional";
}