using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class TinkerableGadgetType : InteractiveGadgetType
{
    public static TinkerableGadgetType Instance { get; } = new();

    private TinkerableGadgetType()
    {
    }

    public override int Id => Global.TinkerableGadgetTypeId;
    public override string GadgetTypeName => "tinkerable";

    public override void InteractWithLemming(Lemming lemming)
    {
    }
}