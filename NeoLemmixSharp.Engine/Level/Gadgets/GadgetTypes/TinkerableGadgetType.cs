namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class TinkerableGadgetType : GadgetType
{
    public static TinkerableGadgetType Instance { get; } = new();

    private TinkerableGadgetType()
    {
    }

    public override int Id => GameConstants.TinkerableGadgetTypeId;
    public override string GadgetTypeName => "tinkerable";
}