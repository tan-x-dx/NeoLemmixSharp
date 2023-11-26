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
        foreach (var t in _filters.AsSpan())
        {
            if (!t.MatchesLemming(lemming))
                return false;
        }

        return true;
    }
}