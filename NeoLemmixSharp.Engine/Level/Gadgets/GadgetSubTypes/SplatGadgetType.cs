using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class SplatGadgetType : InteractiveGadgetType
{
    public static SplatGadgetType Instance { get; } = new();

    private SplatGadgetType()
    {
    }

    public override int Id => Global.SplatGadgetTypeId;
    public override string GadgetTypeName => "splat";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}