using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingHitBoxFilter
{
    public LemmingSolidityType LemmingSolidityType { get; }
    public HitBoxBehaviour HitBoxHint { get; }
    private readonly ILemmingCriterion[] _criteria;
    private readonly IGadgetAction[] _onLemmingEnterActions;
    private readonly IGadgetAction[] _onLemmingPresentActions;
    private readonly IGadgetAction[] _onLemmingExitActions;

    public ReadOnlySpan<IGadgetAction> OnLemmingEnterActions => new(_onLemmingEnterActions);
    public ReadOnlySpan<IGadgetAction> OnLemmingPresentActions => new(_onLemmingPresentActions);
    public ReadOnlySpan<IGadgetAction> OnLemmingExitActions => new(_onLemmingExitActions);

    public LemmingHitBoxFilter(
        LemmingSolidityType lemmingSolidityType,
        HitBoxBehaviour hitBoxHint,
        ILemmingCriterion[] criteria,
        IGadgetAction[] onLemmingEnterActions,
        IGadgetAction[] onLemmingPresentActions,
        IGadgetAction[] onLemmingExitActions)
    {
        if (criteria.Length == 0)
            throw new ArgumentException("Expected at least one criterion");

        LemmingSolidityType = lemmingSolidityType;
        HitBoxHint = hitBoxHint;
        _criteria = criteria;
        _onLemmingEnterActions = onLemmingEnterActions;
        _onLemmingPresentActions = onLemmingPresentActions;
        _onLemmingExitActions = onLemmingExitActions;
    }

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
