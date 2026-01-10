using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.Functional;

public sealed class LevelTimerObserverGadgetInstanceSpecificationData : IGadgetInstanceSpecificationData
{
    public GadgetType GadgetType => GadgetType.LevelTimerObserver;

    public required LevelTimerObservationType ObservationType { get; init; }
    public required ComparisonType ComparisonType { get; init; }
    public required int RequiredValue { get; init; }

    public int CalculateExtraNumberOfBytesNeededForSnapshotting() => 0;
}

[DebuggerDisplay("{OverrideStateName}")]
public sealed class LevelTimerObserverGadgetStateInstanceData : IGadgetStateInstanceData
{
    public required GadgetStateName OverrideStateName { get; init; }

    ReadOnlySpan<GadgetTriggerData> IGadgetStateInstanceData.CustomTriggers => [];
    ReadOnlySpan<GadgetBehaviourData> IGadgetStateInstanceData.CustomBehaviours => [];
    ReadOnlySpan<GadgetTriggerBehaviourLink> IGadgetStateInstanceData.CustomTriggerBehaviourLinks => [];
}
