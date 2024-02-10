namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class GenericGadgetSubType : GadgetSubType
{
    public static readonly GenericGadgetSubType Instance = new();

    private GenericGadgetSubType()
    {
    }

    public override int Id => LevelConstants.GenericGadgetTypeId;
    public override string GadgetTypeName => "generic";
}