using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Triggers;

public abstract class GlobalConditionTrigger : GadgetTrigger
{
    GeneralBehaviour[] _behaviours;

    protected GlobalConditionTrigger(GadgetTriggerName triggerName)
        : base(triggerName)
    {
    }
}
