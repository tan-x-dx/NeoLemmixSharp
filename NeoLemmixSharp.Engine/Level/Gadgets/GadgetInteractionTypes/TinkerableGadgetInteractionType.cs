using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class TinkerableGadgetInteractionType : GadgetInteractionType
{
    public static readonly TinkerableGadgetInteractionType Instance = new();

    private TinkerableGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.TinkerableGadgetTypeId;
    public override string GadgetTypeName => "tinkerable";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        if (!lemming.State.IsDisarmer)
            return NoneAction.Instance;

        //if()
        return NoneAction.Instance;
    }
}