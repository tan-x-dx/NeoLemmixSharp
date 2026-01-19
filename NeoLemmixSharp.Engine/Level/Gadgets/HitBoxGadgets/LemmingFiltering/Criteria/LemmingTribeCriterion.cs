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
        var tribeId = lemming.State.TribeId;
        var tribe = LevelScreen.TribeManager.GetTribe(tribeId);
        return _tribes.Contains(tribe);
    }
}
