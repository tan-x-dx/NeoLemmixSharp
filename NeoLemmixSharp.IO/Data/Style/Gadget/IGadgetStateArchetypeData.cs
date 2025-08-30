using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public interface IGadgetStateArchetypeData
{
    GadgetStateName StateName { get; }

    ReadOnlySpan<GadgetTriggerData> InnateTriggers { get; }
    ReadOnlySpan<GadgetBehaviourData> InnateBehaviours { get; }
    ReadOnlySpan<GadgetTriggerBehaviourLink> TriggerBehaviourLinks { get; }
}
