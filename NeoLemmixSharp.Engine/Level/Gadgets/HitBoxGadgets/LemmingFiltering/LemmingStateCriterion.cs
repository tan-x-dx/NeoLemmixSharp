using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingStateCriterion : ILemmingCriterion
{
    private readonly ILemmingStateChanger[] _allowedLemmingStates;

    public LemmingStateCriterion(StateChangerSet allowedStates)
    {
        Debug.Assert(allowedStates.Count > 0);
        _allowedLemmingStates = allowedStates.ToArray();
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