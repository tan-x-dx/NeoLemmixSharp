using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public interface IGadgetStateInstanceData
{
    GadgetStateName OverrideStateName { get; }

    ReadOnlySpan<GadgetTriggerData> CustomTriggers { get; }
    ReadOnlySpan<GadgetBehaviourData> CustomBehaviours { get; }
    ReadOnlySpan<GadgetTriggerBehaviourLink> CustomTriggerBehaviourLinks { get; }
}
