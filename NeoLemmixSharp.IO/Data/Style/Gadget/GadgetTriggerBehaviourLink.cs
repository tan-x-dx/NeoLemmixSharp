using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public readonly struct GadgetTriggerBehaviourLink(GadgetTriggerName gadgetTriggerName, GadgetBehaviourName gadgetBehaviourName)
{
    public readonly GadgetTriggerName GadgetTriggerName = gadgetTriggerName;
    public readonly GadgetBehaviourName GadgetBehaviourName = gadgetBehaviourName;
}
