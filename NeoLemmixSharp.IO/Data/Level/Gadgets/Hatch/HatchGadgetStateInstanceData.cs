using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets.Hatch;

public sealed class HatchGadgetStateInstanceData : IGadgetStateInstanceData
{
    public required GadgetStateName OverrideStateName { get; init; }
    public required GadgetTriggerData[] CustomTriggers { get; init; }
    public required GadgetBehaviourData[] CustomBehaviours { get; init; }
    public required GadgetTriggerBehaviourLink[] CustomTriggerBehaviourLinks { get; init; }

    ReadOnlySpan<GadgetTriggerData> IGadgetStateInstanceData.CustomTriggers => CustomTriggers;
    ReadOnlySpan<GadgetBehaviourData> IGadgetStateInstanceData.CustomBehaviours => CustomBehaviours;
    ReadOnlySpan<GadgetTriggerBehaviourLink> IGadgetStateInstanceData.CustomTriggerBehaviourLinks => CustomTriggerBehaviourLinks;
}
