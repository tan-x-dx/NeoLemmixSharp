namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class SwitchGadgetBehaviour : GadgetBehaviour
{
    public static readonly SwitchGadgetBehaviour Instance = new();

    private SwitchGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.SwitchGadgetBehaviourId;
    public override string GadgetBehaviourName => "switch";
}