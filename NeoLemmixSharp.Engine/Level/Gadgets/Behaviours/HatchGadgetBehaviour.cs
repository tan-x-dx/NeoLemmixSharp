namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class HatchGadgetBehaviour : GadgetBehaviour
{
    public static readonly HatchGadgetBehaviour Instance = new();

    private HatchGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.HatchGadgetTypeId;
    public override string GadgetTypeName => "hatch";
}