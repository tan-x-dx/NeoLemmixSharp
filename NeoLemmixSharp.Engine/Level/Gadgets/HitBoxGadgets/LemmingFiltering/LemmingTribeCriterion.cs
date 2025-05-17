using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingTribeCriterion : ILemmingCriterion
{
    private readonly Tribe _tribe;

    public LemmingTribeCriterion(Tribe tribe)
    {
        _tribe = tribe;
    }

    public bool LemmingMatchesCriteria(Lemming lemming)
    {
        return lemming.State.TribeAffiliation == _tribe;
    }
}