using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class UpdraftGadgetInteractionType : GadgetInteractionType
{
    public static readonly UpdraftGadgetInteractionType Instance = new();

    private UpdraftGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.UpdraftGadgetTypeId;
    public override string GadgetTypeName => "updraft";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}