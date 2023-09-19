namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class SplatGadgetType : GadgetType
{
    public static SplatGadgetType Instance { get; } = new();

    private SplatGadgetType()
    {
    }

    public override int Id => GameConstants.SplatGadgetTypeId;
    public override string GadgetTypeName => "splat";
}