using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingStateCriterion : ILemmingCriterion
{
    private readonly ILemmingStateChanger[] _allowedLemmingStates;

    public LemmingStateCriterion(ILemmingStateChanger[] allowedLemmingStates)
    {
        _allowedLemmingStates = allowedLemmingStates;
    }

    public bool LemmingMatchesCriteria(Lemming lemming)
    {
        foreach (var lemmingState in _allowedLemmingStates)
        {
            if (!lemmingState.IsApplied(lemming.State))
                return false;
        }

        return true;
    }
}