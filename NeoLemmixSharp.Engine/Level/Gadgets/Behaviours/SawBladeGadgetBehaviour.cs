namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class SawBladeGadgetBehaviour : GadgetBehaviour
{
    public static readonly SawBladeGadgetBehaviour Instance = new();

    private SawBladeGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.SawBladeGadgetTypeId;
    public override string GadgetTypeName => "saw blade";
}