using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class WaterGadgetInteractionType : GadgetInteractionType
{
    public static readonly WaterGadgetInteractionType Instance = new();

    private WaterGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.WaterGadgetTypeId;
    public override string GadgetTypeName => "water";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}