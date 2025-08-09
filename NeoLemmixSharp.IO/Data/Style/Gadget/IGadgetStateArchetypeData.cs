using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public interface IGadgetStateArchetypeData
{
    GadgetStateName StateName { get; }

    GadgetTriggerData[] InnateTriggers { get; }
    GadgetBehaviourData[] InnateBehaviours { get; }
}
