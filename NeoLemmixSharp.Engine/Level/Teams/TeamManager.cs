using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Engine.Level.Teams;

public sealed class TeamManager :
    IPerfectHasher<Team>,
    IItemManager<Team>,
    IDisposable
{
    private readonly Team[] _teams;
    public ReadOnlySpan<Team> AllItems => new(_teams);

    public TeamManager(Team[] teams)
    {
        _teams = teams;
        this.ValidateUniqueIds(new ReadOnlySpan<Team>(_teams));
        Array.Sort(_teams, this);
    }

    public int NumberOfItems => _teams.Length;

    int IPerfectHasher<Team>.Hash(Team item) => item.Id;
    Team IPerfectHasher<Team>.UnHash(int index) => _teams[index];

    public void Dispose()
    {
        new Span<Team>(_teams).Clear();
    }
}
