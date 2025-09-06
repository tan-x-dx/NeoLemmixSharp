using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingActionCriterion : LemmingCriterion
{
    private readonly LemmingActionSet _lemmingActions;

    public LemmingActionCriterion(LemmingActionSet actions)
        : base(LemmingCriteriaType.LemmingAction)
    {
        _lemmingActions = actions;
    }

    public override bool LemmingMatchesCriteria(Lemming lemming) => _lemmingActions.Contains(lemming.CurrentAction);
}
