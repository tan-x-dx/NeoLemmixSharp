namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class MetalGrateGadgetSubType : GadgetSubType
{
    public static readonly MetalGrateGadgetSubType Instance = new();

    private MetalGrateGadgetSubType()
    {
    }

    public override int Id => LevelConstants.MetalGrateGadgetTypeId;
    public override string GadgetTypeName => "metal grate";
}