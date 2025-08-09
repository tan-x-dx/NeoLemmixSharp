using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

[DebuggerDisplay("{StateName}")]
public sealed class HitBoxGadgetStateArchetypeData : IGadgetStateArchetypeData
{
    public required GadgetStateName StateName { get; init; }
    public required Point HitBoxOffset { get; init; }
    public required HitBoxRegionData[] RegionData { get; init; }
    public required AnimationLayerArchetypeData[] AnimationLayerData { get; init; }
    public required GadgetTriggerData[] InnateTriggers { get; init; }
    public required GadgetBehaviourData[] InnateBehaviours { get; init; }
    public required LemmingSolidityType SolidityType { get; init; }
    public required HitBoxInteractionType HitBoxBehaviour { get; init; }

    public required GadgetActionData[] InnateOnLemmingEnterActions { get; init; }
    public required GadgetActionData[] InnateOnLemmingPresentActions { get; init; }
    public required GadgetActionData[] InnateOnLemmingExitActions { get; init; }

    public required HitBoxCriteriaData[] InnateHitBoxCriteria { get; init; }
    public BitArraySet<LemmingCriteriaHasher, BitBuffer32, LemmingCriteria> AllowedCustomHitBoxCriteria { get; } = LemmingCriteriaHasher.CreateBitSet();
}
