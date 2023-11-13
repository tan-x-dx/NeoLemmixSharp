using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class FireGadgetType : InteractiveGadgetType
{
    public static FireGadgetType Instance { get; } = new();

    private FireGadgetType()
    {
    }

    public override int Id => LevelConstants.FireGadgetTypeId;
    public override string GadgetTypeName => "fire";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return VaporiserAction.Instance;
    }
}