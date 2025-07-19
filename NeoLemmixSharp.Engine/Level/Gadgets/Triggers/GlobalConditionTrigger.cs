using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Triggers;

public abstract class GlobalConditionTrigger : GadgetTrigger
{
    protected GlobalConditionTrigger(
        GadgetTriggerName triggerName,
        GeneralBehaviour[] generalBehaviours)
        : base(triggerName, generalBehaviours)
    {
    }
}
