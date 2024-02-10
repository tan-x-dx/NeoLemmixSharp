using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class GenericGadgetInteractionType : GadgetInteractionType
{
    public static readonly GenericGadgetInteractionType Instance = new();

    private GenericGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.GenericGadgetTypeId;
    public override string GadgetTypeName => "generic";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}