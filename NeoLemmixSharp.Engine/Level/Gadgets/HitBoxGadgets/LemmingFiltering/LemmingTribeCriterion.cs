using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingTribeCriterion : LemmingCriterion
{
    private readonly Tribe _tribe;

    public LemmingTribeCriterion(Tribe tribe)
        : base(LemmingCriteria.LemmingTribe)
    {
        _tribe = tribe;
    }

    public override bool LemmingMatchesCriteria(Lemming lemming)
    {
        return lemming.State.TribeAffiliation == _tribe;
    }
}
