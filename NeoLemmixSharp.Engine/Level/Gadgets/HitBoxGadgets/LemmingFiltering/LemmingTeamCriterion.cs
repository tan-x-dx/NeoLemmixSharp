using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingTeamCriterion : ILemmingCriterion
{
    private readonly Team _team;

    public LemmingTeamCriterion(Team team)
    {
        _team = team;
    }

    public bool LemmingMatchesCriteria(Lemming lemming)
    {
        return lemming.State.TeamAffiliation == _team;
    }
}