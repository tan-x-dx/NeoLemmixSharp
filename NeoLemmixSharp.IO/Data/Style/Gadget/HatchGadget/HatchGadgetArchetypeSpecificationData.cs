using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HatchGadget;

[DebuggerDisplay("Hatch - {StyleIdentifier}:{PieceIdentifier}")]
public sealed class HatchGadgetArchetypeSpecificationData : IGadgetArchetypeSpecificationData
{
    public GadgetType GadgetType => GadgetType.HatchGadget;

    public required Point SpawnOffset { get; init; }
    public required HatchGadgetStateArchetypeData[] GadgetStates { get; init; }

    ReadOnlySpan<IGadgetStateArchetypeData> IGadgetArchetypeSpecificationData.AllStates => GadgetStates;

    public int CalculateExtraNumberOfBytesNeededForSnapshotting()
    {
        var result = 0;

        foreach (var state in GadgetStates)
        {
            for (var i = 0; i < state.InnateBehaviours.Length; i++)
            {
                var behaviourType = state.InnateBehaviours[i].GadgetBehaviourType;
                result += GadgetBehaviourTypeHasher.NumberOfSnapshotBytesRequiredForBehaviour(behaviourType);
            }
        }

        return result;
    }
}

[DebuggerDisplay("{StateName}")]
public sealed class HatchGadgetStateArchetypeData : IGadgetStateArchetypeData
{
    public required GadgetStateName StateName { get; init; }
    public required HatchGadgetStateType Type { get; init; }
    public required GadgetTriggerData[] InnateTriggers { get; init; }
    public required GadgetBehaviourData[] InnateBehaviours { get; init; }
    public required GadgetTriggerBehaviourLink[] TriggerBehaviourLinks { get; init; }

    ReadOnlySpan<GadgetTriggerData> IGadgetStateArchetypeData.InnateTriggers => InnateTriggers;
    ReadOnlySpan<GadgetBehaviourData> IGadgetStateArchetypeData.InnateBehaviours => InnateBehaviours;
    ReadOnlySpan<GadgetTriggerBehaviourLink> IGadgetStateArchetypeData.TriggerBehaviourLinks => TriggerBehaviourLinks;
}
