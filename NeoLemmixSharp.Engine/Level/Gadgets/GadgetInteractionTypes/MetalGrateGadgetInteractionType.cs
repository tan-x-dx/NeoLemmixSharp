using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class MetalGrateGadgetInteractionType : GadgetInteractionType
{
    public static readonly MetalGrateGadgetInteractionType Instance = new();

    private MetalGrateGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.MetalGrateGadgetTypeId;
    public override string GadgetTypeName => "metal grate";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}