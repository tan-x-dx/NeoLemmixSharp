using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public sealed class SwitchGadgetInteractionType : GadgetInteractionType
{
    public static readonly SwitchGadgetInteractionType Instance = new();

    private SwitchGadgetInteractionType()
    {
    }

    public override int Id => LevelConstants.SwitchGadgetTypeId;
    public override string GadgetTypeName => "switch";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}