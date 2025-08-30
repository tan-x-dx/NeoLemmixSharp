using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Hatch;

[DebuggerDisplay("{StateName}")]
public sealed class HatchGadgetStateArchetypeData : IGadgetStateArchetypeData
{
    public required GadgetStateName StateName { get; init; }
    public required GadgetTriggerData[] InnateTriggers { get; init; }
    public required GadgetBehaviourData[] InnateBehaviours { get; init; }
    public required GadgetTriggerBehaviourLink[] TriggerBehaviourLinks { get; init; }

    ReadOnlySpan<GadgetTriggerData> IGadgetStateArchetypeData.InnateTriggers => InnateTriggers;
    ReadOnlySpan<GadgetBehaviourData> IGadgetStateArchetypeData.InnateBehaviours => InnateBehaviours;
    ReadOnlySpan<GadgetTriggerBehaviourLink> IGadgetStateArchetypeData.TriggerBehaviourLinks => TriggerBehaviourLinks;
}
