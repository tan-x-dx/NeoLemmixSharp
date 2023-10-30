using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public sealed class UpdraftGadgetType : InteractiveGadgetType
{
    public static UpdraftGadgetType Instance { get; } = new();

    private UpdraftGadgetType()
    {
    }

    public override int Id => LevelConstants.UpdraftGadgetTypeId;
    public override string GadgetTypeName => "updraft";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}