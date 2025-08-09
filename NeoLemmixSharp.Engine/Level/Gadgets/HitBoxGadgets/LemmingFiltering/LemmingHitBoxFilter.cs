using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingHitBoxFilter : GadgetTrigger
{
    private readonly LemmingCriterion[] _criteria;
    private readonly int[] _onLemmingHitBehaviourIds;
    private readonly int[] _onLemmingEnterBehaviourIds;
    private readonly int[] _onLemmingPresentBehaviourIds;
    private readonly int[] _onLemmingExitBehaviourIds;

    public LemmingSolidityType LemmingSolidityType { get; }
    public HitBoxInteractionType HitBoxBehaviour { get; }

    public ReadOnlySpan<int> OnLemmingHitBehaviourIds => new(_onLemmingHitBehaviourIds);
    public ReadOnlySpan<int> OnLemmingEnterBehaviourIds => new(_onLemmingEnterBehaviourIds);
    public ReadOnlySpan<int> OnLemmingPresentBehaviourIds => new(_onLemmingPresentBehaviourIds);
    public ReadOnlySpan<int> OnLemmingExitBehaviourIds => new(_onLemmingExitBehaviourIds);

    public LemmingHitBoxFilter(
        int[] gadgetBehaviourIds,
        LemmingSolidityType lemmingSolidityType,
        HitBoxInteractionType hitBoxBehaviour,
        LemmingCriterion[] criteria,
        int[] onLemmingHitBehaviourIds,
        int[] onLemmingEnterBehaviourIds,
        int[] onLemmingPresentBehaviourIds,
        int[] onLemmingExitBehaviourIds)
        : base(gadgetBehaviourIds)
    {
        LemmingSolidityType = lemmingSolidityType;
        HitBoxBehaviour = hitBoxBehaviour;
        _criteria = criteria;
        _onLemmingHitBehaviourIds = onLemmingHitBehaviourIds;
        _onLemmingEnterBehaviourIds = onLemmingEnterBehaviourIds;
        _onLemmingPresentBehaviourIds = onLemmingPresentBehaviourIds;
        _onLemmingExitBehaviourIds = onLemmingExitBehaviourIds;
    }

    public override void Tick()
    {
    }

    [Pure]
    public bool MatchesLemming(Lemming lemming)
    {
        for (var i = 0; i < _criteria.Length; i++)
        {
            if (!_criteria[i].LemmingMatchesCriteria(lemming))
                return false;
        }

        return true;
    }
}
