using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class SwitchGadgetType : InteractiveGadgetType
{
    public static readonly SwitchGadgetType Instance = new();

    private SwitchGadgetType()
    {
    }

    public override int Id => LevelConstants.SwitchGadgetTypeId;
    public override string GadgetTypeName => "switch";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}