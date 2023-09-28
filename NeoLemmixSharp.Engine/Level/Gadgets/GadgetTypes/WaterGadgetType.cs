using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class WaterGadgetType : InteractiveGadgetType
{
    public static WaterGadgetType Instance { get; } = new();

    private WaterGadgetType()
    {
    }

    public override int Id => Global.WaterGadgetTypeId;
    public override string GadgetTypeName => "water";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}