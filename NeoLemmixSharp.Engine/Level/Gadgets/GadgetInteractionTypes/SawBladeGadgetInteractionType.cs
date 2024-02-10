using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class SawBladeGadgetInteractionType : GadgetInteractionType
{
    public static readonly SawBladeGadgetInteractionType Instance = new();

    private SawBladeGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.SawBladeGadgetTypeId;
    public override string GadgetTypeName => "saw blade";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}