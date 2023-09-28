using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class MetalGrateGadgetType : InteractiveGadgetType
{
    public static MetalGrateGadgetType Instance { get; } = new();

    private MetalGrateGadgetType()
    {
    }

    public override int Id => Global.MetalGrateGadgetTypeId;
    public override string GadgetTypeName => "metal grate";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}