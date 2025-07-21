using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Triggers;

public abstract class GadgetLinkTrigger : GadgetTrigger
{
    protected GadgetLinkTrigger(
        GadgetTriggerName triggerName,
        GadgetBehaviour[] gadgetBehaviours)
        : base(triggerName, gadgetBehaviours)
    {
    }

    public abstract void ReactToSignal(bool signal);
}
