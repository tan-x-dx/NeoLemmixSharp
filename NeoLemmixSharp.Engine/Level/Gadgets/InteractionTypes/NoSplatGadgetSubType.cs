namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class NoSplatGadgetSubType : GadgetSubType
{
    public static readonly NoSplatGadgetSubType Instance = new();

    private NoSplatGadgetSubType()
    {
    }

    public override int Id => LevelConstants.NoSplatGadgetTypeId;
    public override string GadgetTypeName => "no splat";
}