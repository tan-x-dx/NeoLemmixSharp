namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class GenericGadgetBehaviour : GadgetBehaviour
{
    public static readonly GenericGadgetBehaviour Instance = new();

    private GenericGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.GenericGadgetBehaviourId;
    public override string GadgetBehaviourName => "generic";
}