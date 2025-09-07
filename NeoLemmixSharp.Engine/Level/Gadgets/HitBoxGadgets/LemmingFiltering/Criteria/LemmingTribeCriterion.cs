using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingTribeCriterion : LemmingCriterion
{
    private readonly TribeSet _tribes;

    public LemmingTribeCriterion(TribeSet tribes)
        : base(LemmingCriteriaType.LemmingTribe)
    {
        _tribes = tribes;
    }

    public override bool LemmingMatchesCriteria(Lemming lemming)
    {
        return _tribes.Contains(lemming.State.TribeAffiliation);
    }
}
