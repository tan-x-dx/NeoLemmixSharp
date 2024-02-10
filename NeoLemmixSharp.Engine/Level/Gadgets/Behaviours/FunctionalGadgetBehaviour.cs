namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class FunctionalGadgetBehaviour : GadgetBehaviour
{
    public static readonly FunctionalGadgetBehaviour Instance = new();

    private FunctionalGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.FunctionalGadgetTypeId;
    public override string GadgetTypeName => "functional";
}