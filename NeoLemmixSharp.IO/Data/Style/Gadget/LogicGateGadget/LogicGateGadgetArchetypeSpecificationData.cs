using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.LogicGateGadget;

[DebuggerDisplay("Logic Gate - {GadgetType} - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class LogicGateGadgetArchetypeSpecificationData : IGadgetArchetypeSpecificationData
{
    public GadgetType GadgetType => GadgetType.LogicGate;
    public required LogicGateStateArchetypeData[] GadgetStates { get; init; }

    ReadOnlySpan<IGadgetStateArchetypeData> IGadgetArchetypeSpecificationData.AllStates => GadgetStates;
}

[DebuggerDisplay("{StateName}")]
public sealed class LogicGateStateArchetypeData : IGadgetStateArchetypeData
{
    public required GadgetStateName StateName { get; init; }
    public required GadgetTriggerData[] InnateTriggers { get; init; }
    public required GadgetBehaviourData[] InnateBehaviours { get; init; }
    public required GadgetTriggerBehaviourLink[] TriggerBehaviourLinks { get; init; }

    ReadOnlySpan<GadgetTriggerData> IGadgetStateArchetypeData.InnateTriggers => InnateTriggers;
    ReadOnlySpan<GadgetBehaviourData> IGadgetStateArchetypeData.InnateBehaviours => InnateBehaviours;
    ReadOnlySpan<GadgetTriggerBehaviourLink> IGadgetStateArchetypeData.TriggerBehaviourLinks => TriggerBehaviourLinks;
}
