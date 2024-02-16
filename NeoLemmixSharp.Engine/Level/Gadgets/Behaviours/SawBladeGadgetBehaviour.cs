namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class SawBladeGadgetBehaviour : GadgetBehaviour
{
    public static readonly SawBladeGadgetBehaviour Instance = new();

    private SawBladeGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.SawBladeGadgetBehaviourId;
    public override string GadgetBehaviourName => "saw blade";
}