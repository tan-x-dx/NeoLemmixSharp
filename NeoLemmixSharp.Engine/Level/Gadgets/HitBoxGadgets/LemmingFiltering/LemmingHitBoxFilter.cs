using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingHitBoxFilter : GadgetTrigger
{
    private readonly LemmingCriterion[] _lemmingCriteria;
    private readonly GadgetBehaviour[] _onLemmingHitBehaviours;
    private readonly GadgetBehaviour[] _onLemmingEnterBehaviours;
    private readonly GadgetBehaviour[] _onLemmingPresentBehaviours;
    private readonly GadgetBehaviour[] _onLemmingExitBehaviours;

    public LemmingSolidityType LemmingSolidityType { get; }
    public HitBoxInteractionType HitBoxBehaviour { get; }

    public ReadOnlySpan<GadgetBehaviour> OnLemmingHitBehaviours => new(_onLemmingHitBehaviours);
    public ReadOnlySpan<GadgetBehaviour> OnLemmingEnterBehaviours => new(_onLemmingEnterBehaviours);
    public ReadOnlySpan<GadgetBehaviour> OnLemmingPresentBehaviours => new(_onLemmingPresentBehaviours);
    public ReadOnlySpan<GadgetBehaviour> OnLemmingExitBehaviours => new(_onLemmingExitBehaviours);

    public LemmingHitBoxFilter(
        LemmingSolidityType lemmingSolidityType,
        HitBoxInteractionType hitBoxBehaviour,
        LemmingCriterion[] lemmingCriteria,
        GadgetBehaviour[] onLemmingHitBehaviours,
        GadgetBehaviour[] onLemmingEnterBehaviours,
        GadgetBehaviour[] onLemmingPresentBehaviours,
        GadgetBehaviour[] onLemmingExitBehaviours)
        : base(GadgetTriggerType.LemmingHitBoxTrigger)
    {
        LemmingSolidityType = lemmingSolidityType;
        HitBoxBehaviour = hitBoxBehaviour;
        _lemmingCriteria = lemmingCriteria;
        _onLemmingHitBehaviours = onLemmingHitBehaviours;
        _onLemmingEnterBehaviours = onLemmingEnterBehaviours;
        _onLemmingPresentBehaviours = onLemmingPresentBehaviours;
        _onLemmingExitBehaviours = onLemmingExitBehaviours;
    }

    public override void DetectTrigger()
    {
        // Do nothing - this type looks for lemmings specifically
    }

    [Pure]
    public bool MatchesLemming(Lemming lemming)
    {
        foreach (LemmingCriterion criterion in _lemmingCriteria)
        {
            if (!criterion.LemmingMatchesCriteria(lemming))
                return false;
        }

        return true;
    }
}
