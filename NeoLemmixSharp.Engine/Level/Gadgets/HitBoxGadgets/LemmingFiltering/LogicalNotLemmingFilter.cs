using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LogicalNotLemmingFilter : ILemmingFilter
{
    private readonly ILemmingFilter _filter;

    public LogicalNotLemmingFilter(ILemmingFilter filter)
    {
        _filter = filter;
    }

    public bool MatchesLemming(Lemming lemming)
    {
        return !_filter.MatchesLemming(lemming);
    }
}