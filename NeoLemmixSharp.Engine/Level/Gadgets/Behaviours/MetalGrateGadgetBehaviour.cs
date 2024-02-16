namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class MetalGrateGadgetBehaviour : GadgetBehaviour
{
    public static readonly MetalGrateGadgetBehaviour Instance = new();

    private MetalGrateGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.MetalGrateGadgetBehaviourId;
    public override string GadgetBehaviourName => "metal grate";
}