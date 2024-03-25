using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingStateFilter : ILemmingFilter
{
    private readonly ILemmingStateChanger[] _allowedLemmingStates;

    public LemmingStateFilter(ILemmingStateChanger[] allowedLemmingStates)
    {
        _allowedLemmingStates = allowedLemmingStates;
    }

    public bool MatchesLemming(Lemming lemming)
    {
        foreach (var lemmingState in _allowedLemmingStates)
        {
            if (lemmingState.IsApplied(lemming.State))
                return true;
        }

        return false;
    }
}