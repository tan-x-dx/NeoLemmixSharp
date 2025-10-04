using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.HatchGadget;

public sealed class HatchGadgetInstanceSpecificationData : IGadgetInstanceSpecificationData
{
    public GadgetType GadgetType => GadgetType.HatchGadget;
    public required int InitialStateId { get; init; }
    public required HatchGadgetStateInstanceData[] GadgetStates { get; init; }

    public required int HatchGroupId { get; init; }
    public required int TribeId { get; init; }
    public required uint RawStateData { get; init; }
    public required int NumberOfLemmingsToRelease { get; init; }
}

[DebuggerDisplay("{OverrideStateName}")]
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
