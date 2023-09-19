namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class GenericGadgetType : GadgetType
{
    public static GenericGadgetType Instance { get; } = new();

    private GenericGadgetType()
    {
    }

    public override int Id => GameConstants.GenericGadgetTypeId;
    public override string GadgetTypeName => "generic";
}