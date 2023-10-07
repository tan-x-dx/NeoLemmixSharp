using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingStateFilter : ILemmingFilter
{
    private readonly ILemmingStateChanger _lemmingStateChanger;

    public LemmingStateFilter(ILemmingStateChanger lemmingStateChanger)
    {
        _lemmingStateChanger = lemmingStateChanger;
    }

    public bool MatchesLemming(Lemming lemming)
    {
        return _lemmingStateChanger.IsApplied(lemming.State);
    }
}