using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadgetGadget;

public sealed class HitBoxFilterData
{
    public required HitBoxFilterName HitBoxFilterName { get; init; }
    public required LemmingSolidityType SolidityType { get; init; }
    public required HitBoxInteractionType HitBoxBehaviour { get; init; }
    public required HitBoxCriteriaData[] HitBoxCriteria { get; init; }
    public required GadgetBehaviourData[] OnLemmingHitBehaviours { get; init; }
    public required GadgetBehaviourData[] OnLemmingEnterBehaviours { get; init; }
    public required GadgetBehaviourData[] OnLemmingPresentBehaviours { get; init; }
    public required GadgetBehaviourData[] OnLemmingExitBehaviours { get; init; }
}
