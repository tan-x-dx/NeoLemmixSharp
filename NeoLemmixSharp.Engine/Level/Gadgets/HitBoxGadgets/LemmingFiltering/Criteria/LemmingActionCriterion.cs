using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingActionCriterion : LemmingCriterion
{
    private readonly LemmingActionSet _lemmingActions;

    public LemmingActionCriterion(LemmingActionSet actions)
        : base(LemmingCriteria.LemmingAction)
    {
        _lemmingActions = actions;
    }

    public override bool LemmingMatchesCriteria(Lemming lemming) => _lemmingActions.Contains(lemming.CurrentAction);
}
