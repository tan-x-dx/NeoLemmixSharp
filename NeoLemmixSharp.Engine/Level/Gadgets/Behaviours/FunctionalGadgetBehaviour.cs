namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class FunctionalGadgetBehaviour : GadgetBehaviour
{
    public static readonly FunctionalGadgetBehaviour Instance = new();

    private FunctionalGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.FunctionalGadgetBehaviourId;
    public override string GadgetBehaviourName => "functional";
}