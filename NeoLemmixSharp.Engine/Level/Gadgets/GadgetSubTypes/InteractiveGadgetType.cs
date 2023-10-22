using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public abstract class InteractiveGadgetType : GadgetSubType
{
    public abstract LemmingAction InteractWithLemming(Lemming lemming);
}