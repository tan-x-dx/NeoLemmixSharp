using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingHitBoxFilter
{
    private readonly LemmingCriterion[] _criteria;
    private readonly LemmingBehaviour[] _onLemmingEnterActions;
    private readonly LemmingBehaviour[] _onLemmingPresentActions;
    private readonly LemmingBehaviour[] _onLemmingExitActions;

    public LemmingSolidityType LemmingSolidityType { get; }
    public HitBoxBehaviour HitBoxBehaviour { get; }

    public ReadOnlySpan<LemmingBehaviour> OnLemmingEnterActions => new(_onLemmingEnterActions);
    public ReadOnlySpan<LemmingBehaviour> OnLemmingPresentActions => new(_onLemmingPresentActions);
    public ReadOnlySpan<LemmingBehaviour> OnLemmingExitActions => new(_onLemmingExitActions);

    public LemmingHitBoxFilter(
        LemmingSolidityType lemmingSolidityType,
        HitBoxBehaviour hitBoxBehaviour,
        LemmingCriterion[] criteria,
        LemmingBehaviour[] onLemmingEnterActions,
        LemmingBehaviour[] onLemmingPresentActions,
        LemmingBehaviour[] onLemmingExitActions)
    {
        LemmingSolidityType = lemmingSolidityType;
        HitBoxBehaviour = hitBoxBehaviour;
        _criteria = criteria;
        _onLemmingEnterActions = onLemmingEnterActions;
        _onLemmingPresentActions = onLemmingPresentActions;
        _onLemmingExitActions = onLemmingExitActions;
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
