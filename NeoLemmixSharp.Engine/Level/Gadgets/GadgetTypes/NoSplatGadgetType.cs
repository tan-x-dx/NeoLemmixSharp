namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class NoSplatGadgetType : GadgetType
{
    public static NoSplatGadgetType Instance { get; } = new();

    private NoSplatGadgetType()
    {
    }

    public override int Id => GameConstants.NoSplatGadgetTypeId;
    public override string GadgetTypeName => "no splat";
}