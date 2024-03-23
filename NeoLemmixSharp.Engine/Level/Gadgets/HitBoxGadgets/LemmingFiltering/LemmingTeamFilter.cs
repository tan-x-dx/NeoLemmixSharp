using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingTeamFilter : ILemmingFilter
{
    private readonly Team _team;

    public LemmingTeamFilter(Team team)
    {
        _team = team;
    }

    public bool MatchesLemming(Lemming lemming)
    {
        return lemming.State.TeamAffiliation == _team;
    }
}