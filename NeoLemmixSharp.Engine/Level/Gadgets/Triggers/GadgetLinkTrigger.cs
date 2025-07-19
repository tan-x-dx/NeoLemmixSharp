using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Triggers;

public abstract class GadgetLinkTrigger : GadgetTrigger
{
    protected GadgetLinkTrigger(
        GadgetTriggerName triggerName,
        GeneralBehaviour[] generalBehaviours)
        : base(triggerName, generalBehaviours)
    {
    }

    public abstract void ReactToSignal(bool signal);
}
