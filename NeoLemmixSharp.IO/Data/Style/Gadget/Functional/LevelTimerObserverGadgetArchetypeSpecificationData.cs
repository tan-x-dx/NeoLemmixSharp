using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Functional;

[DebuggerDisplay("LevelTimerObserver - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class LevelTimerObserverGadgetArchetypeSpecificationData : IGadgetArchetypeSpecificationData
{
    public GadgetType GadgetType => GadgetType.LevelTimerObserver;

    ReadOnlySpan<IGadgetStateArchetypeData> IGadgetArchetypeSpecificationData.AllStates => [];

    public int CalculateExtraNumberOfBytesNeededForSnapshotting() => sizeof(int);
}

[DebuggerDisplay("{StateName}")]
public sealed class LevelTimerObserverGadgetStateArchetypeData : IGadgetStateArchetypeData
{
    public required GadgetStateName StateName { get; init; }
    public required GadgetTriggerData[] InnateTriggers { get; init; }
    public required GadgetBehaviourData[] InnateBehaviours { get; init; }
    public required GadgetTriggerBehaviourLink[] TriggerBehaviourLinks { get; init; }

    ReadOnlySpan<GadgetTriggerData> IGadgetStateArchetypeData.InnateTriggers => InnateTriggers;
    ReadOnlySpan<GadgetBehaviourData> IGadgetStateArchetypeData.InnateBehaviours => InnateBehaviours;
    ReadOnlySpan<GadgetTriggerBehaviourLink> IGadgetStateArchetypeData.TriggerBehaviourLinks => TriggerBehaviourLinks;
}
