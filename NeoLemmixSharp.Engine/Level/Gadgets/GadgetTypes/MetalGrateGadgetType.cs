namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class MetalGrateGadgetType : GadgetType
{
    public static MetalGrateGadgetType Instance { get; } = new();

    private MetalGrateGadgetType()
    {
    }

    public override int Id => GameConstants.MetalGrateGadgetTypeId;
    public override string GadgetTypeName => "metal grate";
}