using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingTribeCriterion : LemmingCriterion
{
    private readonly TribeSet _tribes;

    public LemmingTribeCriterion(TribeSet tribes)
        : base(LemmingCriteria.LemmingTribe)
    {
        _tribes = tribes;
    }

    public override bool LemmingMatchesCriteria(Lemming lemming)
    {
        return _tribes.Contains(lemming.State.TribeAffiliation);
    }
}
