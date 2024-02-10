using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;

public abstract class GadgetInteractionType : GadgetSubType
{
    public abstract LemmingAction InteractWithLemming(Lemming lemming);
}