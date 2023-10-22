using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class TinkerableGadgetType : InteractiveGadgetType
{
    public static TinkerableGadgetType Instance { get; } = new();

    private TinkerableGadgetType()
    {
    }

    public override int Id => Global.TinkerableGadgetTypeId;
    public override string GadgetTypeName => "tinkerable";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        if (!lemming.State.IsDisarmer)
            return NoneAction.Instance;

        //if()
        return NoneAction.Instance;
    }
}