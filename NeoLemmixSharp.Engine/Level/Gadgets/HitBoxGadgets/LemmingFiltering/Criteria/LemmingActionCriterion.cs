using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingActionCriterion : LemmingCriterion
{
    private readonly LemmingActionTypeSet _lemmingActionTypes;

    public LemmingActionCriterion(LemmingActionTypeSet lemmingActionTypes)
        : base(LemmingCriteriaType.LemmingAction)
    {
        _lemmingActionTypes = lemmingActionTypes;
    }

    public override bool LemmingMatchesCriteria(Lemming lemming) => _lemmingActionTypes.Contains(lemming.CurrentActionType);
}
