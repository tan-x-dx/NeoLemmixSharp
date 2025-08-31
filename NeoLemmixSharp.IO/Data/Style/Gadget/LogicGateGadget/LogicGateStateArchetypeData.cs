using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.LogicGateGadget;

[DebuggerDisplay("{StateName}")]
public sealed class LogicGateStateArchetypeData : IGadgetStateArchetypeData
{
    public required GadgetStateName StateName { get; init; }
    public required GadgetBehaviourData[] GadgetBehaviourData { get; init; }

    public ReadOnlySpan<GadgetTriggerData> InnateTriggers => [];
    public ReadOnlySpan<GadgetBehaviourData> InnateBehaviours => GadgetBehaviourData;
    public ReadOnlySpan<GadgetTriggerBehaviourLink> TriggerBehaviourLinks => [];
}
