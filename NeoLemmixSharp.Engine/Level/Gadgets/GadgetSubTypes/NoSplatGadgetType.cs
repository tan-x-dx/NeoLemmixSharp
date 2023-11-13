using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class NoSplatGadgetType : InteractiveGadgetType
{
    public static NoSplatGadgetType Instance { get; } = new();

    private NoSplatGadgetType()
    {
    }

    public override int Id => LevelConstants.NoSplatGadgetTypeId;
    public override string GadgetTypeName => "no splat";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}