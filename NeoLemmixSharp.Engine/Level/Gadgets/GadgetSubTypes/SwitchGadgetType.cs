using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class SwitchGadgetType : InteractiveGadgetType
{
    public static SwitchGadgetType Instance { get; } = new();

    private SwitchGadgetType()
    {
    }

    public override int Id => Global.SwitchGadgetTypeId;
    public override string GadgetTypeName => "switch";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}