using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingHitBoxFilter
{
    private readonly LemmingCriterion[] _criteria;
    private readonly GadgetAction[] _onLemmingEnterActions;
    private readonly GadgetAction[] _onLemmingPresentActions;
    private readonly GadgetAction[] _onLemmingExitActions;

    public LemmingSolidityType LemmingSolidityType { get; }
    public HitBoxBehaviour HitBoxBehaviour { get; }

    public ReadOnlySpan<GadgetAction> OnLemmingEnterActions => new(_onLemmingEnterActions);
    public ReadOnlySpan<GadgetAction> OnLemmingPresentActions => new(_onLemmingPresentActions);
    public ReadOnlySpan<GadgetAction> OnLemmingExitActions => new(_onLemmingExitActions);

    public LemmingHitBoxFilter(
        LemmingSolidityType lemmingSolidityType,
        HitBoxBehaviour hitBoxBehaviour,
        LemmingCriterion[] criteria,
        GadgetAction[] onLemmingEnterActions,
        GadgetAction[] onLemmingPresentActions,
        GadgetAction[] onLemmingExitActions)
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
