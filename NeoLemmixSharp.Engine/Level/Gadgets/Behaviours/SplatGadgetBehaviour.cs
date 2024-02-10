namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class SplatGadgetBehaviour : GadgetBehaviour
{
    public static readonly SplatGadgetBehaviour Instance = new();

    private SplatGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.SplatGadgetTypeId;
    public override string GadgetTypeName => "splat";
}