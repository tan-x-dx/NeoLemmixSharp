namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class SplatGadgetSubType : GadgetSubType
{
    public static readonly SplatGadgetSubType Instance = new();

    private SplatGadgetSubType()
    {
    }

    public override int Id => LevelConstants.SplatGadgetTypeId;
    public override string GadgetTypeName => "splat";
}