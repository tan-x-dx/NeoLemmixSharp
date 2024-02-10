using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class FireGadgetInteractionType : GadgetInteractionType
{
    public static readonly FireGadgetInteractionType Instance = new();

    private FireGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.FireGadgetTypeId;
    public override string GadgetTypeName => "fire";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return VaporiserAction.Instance;
    }
}