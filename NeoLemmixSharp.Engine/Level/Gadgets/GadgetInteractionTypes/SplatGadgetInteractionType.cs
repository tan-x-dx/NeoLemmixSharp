using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class SplatGadgetInteractionType : GadgetInteractionType
{
    public static readonly SplatGadgetInteractionType Instance = new();

    private SplatGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.SplatGadgetTypeId;
    public override string GadgetTypeName => "splat";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}