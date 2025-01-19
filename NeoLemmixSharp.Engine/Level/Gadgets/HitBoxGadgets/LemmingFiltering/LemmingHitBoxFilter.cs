using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingHitBoxFilter
{
    private readonly ILemmingCriterion[] _criteria;
    private readonly IGadgetAction[] _onLemmingEnterActions;
    private readonly IGadgetAction[] _onLemmingPresentActions;
    private readonly IGadgetAction[] _onLemmingExitActions;

    public LemmingSolidityType LemmingSolidityType { get; }
    public HitBoxBehaviour HitBoxBehaviour { get; }

    public ReadOnlySpan<IGadgetAction> OnLemmingEnterActions => new(_onLemmingEnterActions);
    public ReadOnlySpan<IGadgetAction> OnLemmingPresentActions => new(_onLemmingPresentActions);
    public ReadOnlySpan<IGadgetAction> OnLemmingExitActions => new(_onLemmingExitActions);

    public LemmingHitBoxFilter(
        LemmingSolidityType lemmingSolidityType,
        HitBoxBehaviour hitBoxBehaviour,
        ILemmingCriterion[] criteria,
        IGadgetAction[] onLemmingEnterActions,
        IGadgetAction[] onLemmingPresentActions,
        IGadgetAction[] onLemmingExitActions)
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
