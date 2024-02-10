namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class NoSplatGadgetBehaviour : GadgetBehaviour
{
    public static readonly NoSplatGadgetBehaviour Instance = new();

    private NoSplatGadgetBehaviour()
    {
    }

    public override int Id => LevelConstants.NoSplatGadgetTypeId;
    public override string GadgetTypeName => "no splat";
}