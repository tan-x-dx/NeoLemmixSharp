using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

public sealed class HitBoxFilterData
{
    public required GadgetTriggerName HitBoxFilterName { get; init; }
    public required LemmingSolidityType SolidityType { get; init; }
    public required HitBoxInteractionType HitBoxBehaviour { get; init; }
    public required HitBoxCriteriaData[] HitBoxCriteria { get; init; }
    public required GadgetBehaviourData[] OnLemmingHitBehaviours { get; init; }
    public required GadgetBehaviourData[] OnLemmingEnterBehaviours { get; init; }
    public required GadgetBehaviourData[] OnLemmingPresentBehaviours { get; init; }
    public required GadgetBehaviourData[] OnLemmingExitBehaviours { get; init; }

    public int CalculateExtraNumberOfBytesNeededForSnapshotting()
    {
        var result = 0;

        result += CalculateExtraNumberOfBytesNeededForSnapshotting(OnLemmingHitBehaviours);
        result += CalculateExtraNumberOfBytesNeededForSnapshotting(OnLemmingEnterBehaviours);
        result += CalculateExtraNumberOfBytesNeededForSnapshotting(OnLemmingPresentBehaviours);
        result += CalculateExtraNumberOfBytesNeededForSnapshotting(OnLemmingExitBehaviours);

        return result;
    }

    private static int CalculateExtraNumberOfBytesNeededForSnapshotting(ReadOnlySpan<GadgetBehaviourData> gadgetBehaviours)
    {
        var result = 0;

        for (var i = 0; i < gadgetBehaviours.Length; i++)
        {
            var gadgetBehaviourType = gadgetBehaviours[i].GadgetBehaviourType;

            result += GadgetBehaviourTypeHasher.NumberOfSnapshotBytesRequiredForBehaviour(gadgetBehaviourType);
        }

        return result;
    }
}
