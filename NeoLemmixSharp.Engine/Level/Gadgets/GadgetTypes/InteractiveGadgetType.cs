using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public abstract class InteractiveGadgetType : GadgetType
{
    public abstract void InteractWithLemming(Lemming lemming);
}