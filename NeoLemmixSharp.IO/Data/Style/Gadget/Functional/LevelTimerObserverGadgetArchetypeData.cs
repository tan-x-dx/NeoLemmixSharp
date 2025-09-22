using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Functional;

[DebuggerDisplay("LevelTimerObserver - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class LevelTimerObserverGadgetArchetypeData : IGadgetArchetypeData
{
    public GadgetType GadgetType => GadgetType.LevelTimerObserver;
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required GadgetName GadgetName { get; init; }
    public required Size BaseSpriteSize { get; init; }
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
