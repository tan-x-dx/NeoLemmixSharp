namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class UpdraftGadgetBehaviour : GadgetBehaviour
{
    public static readonly UpdraftGadgetBehaviour Instance = new();

    private UpdraftGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.UpdraftGadgetTypeId;
    public override string GadgetTypeName => "updraft";
}