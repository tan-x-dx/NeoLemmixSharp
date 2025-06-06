using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingStateCriterion : LemmingCriterion
{
    private readonly ILemmingState[] _allowedLemmingStates;

    public LemmingStateCriterion(LemmingStateSet allowedStates)
        :base(LemmingCriteria.LemmingState)
    {
        Debug.Assert(allowedStates.Count > 0);
        _allowedLemmingStates = allowedStates.ToArray();
    }

    public override bool LemmingMatchesCriteria(Lemming lemming)
    {
        foreach (var lemmingState in _allowedLemmingStates)
        {
            if (!lemmingState.IsApplied(lemming.State))
                return false;
        }

        return true;
    }
}
