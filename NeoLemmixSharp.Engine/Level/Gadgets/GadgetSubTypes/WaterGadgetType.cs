using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class WaterGadgetType : InteractiveGadgetType
{
    public static readonly WaterGadgetType Instance = new();

    private WaterGadgetType()
    {
    }

    public override int Id => LevelConstants.WaterGadgetTypeId;
    public override string GadgetTypeName => "water";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}