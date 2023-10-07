using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LogicalAndLemmingFilter : ILemmingFilter
{
    private readonly ILemmingFilter[] _filters;

    public LogicalAndLemmingFilter(ILemmingFilter[] filters)
    {
        _filters = filters;
    }

    public bool MatchesLemming(Lemming lemming)
    {
        for (var i = 0; i < _filters.Length; i++)
        {
            if (!_filters[i].MatchesLemming(lemming))
                return false;
        }

        return true;
    }
}