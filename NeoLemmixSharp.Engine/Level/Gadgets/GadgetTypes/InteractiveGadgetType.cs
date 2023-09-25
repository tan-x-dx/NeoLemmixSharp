using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public abstract class InteractiveGadgetType : GadgetType
{
    public abstract LemmingAction InteractWithLemming(Lemming lemming);
}