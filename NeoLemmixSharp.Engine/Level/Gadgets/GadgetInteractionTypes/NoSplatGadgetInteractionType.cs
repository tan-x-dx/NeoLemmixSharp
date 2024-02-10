using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class NoSplatGadgetInteractionType : GadgetInteractionType
{
    public static readonly NoSplatGadgetInteractionType Instance = new();

    private NoSplatGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.NoSplatGadgetTypeId;
    public override string GadgetTypeName => "no splat";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}