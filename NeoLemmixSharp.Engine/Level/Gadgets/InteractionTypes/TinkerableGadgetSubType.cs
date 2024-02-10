namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class TinkerableGadgetSubType : GadgetSubType
{
    public static readonly TinkerableGadgetSubType Instance = new();

    private TinkerableGadgetSubType()
    {
    }

    public override int Id => LevelConstants.TinkerableGadgetTypeId;
    public override string GadgetTypeName => "tinkerable";
}