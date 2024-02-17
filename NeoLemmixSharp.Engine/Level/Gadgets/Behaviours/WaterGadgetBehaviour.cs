namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class WaterGadgetBehaviour : GadgetBehaviour
{
    public static readonly WaterGadgetBehaviour Instance = new();

    private WaterGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.WaterGadgetBehaviourId;
    public override string GadgetBehaviourName => "water";
}