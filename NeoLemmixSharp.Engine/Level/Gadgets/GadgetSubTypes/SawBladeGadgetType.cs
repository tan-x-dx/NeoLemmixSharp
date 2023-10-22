using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class SawBladeGadgetType : InteractiveGadgetType
{
    public static SawBladeGadgetType Instance { get; } = new();

    private SawBladeGadgetType()
    {
    }

    public override int Id => Global.SawBladeGadgetTypeId;
    public override string GadgetTypeName => "saw blade";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}