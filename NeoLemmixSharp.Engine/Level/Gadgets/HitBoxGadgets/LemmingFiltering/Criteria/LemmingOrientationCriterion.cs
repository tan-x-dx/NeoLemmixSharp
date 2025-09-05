using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingOrientationCriterion : LemmingCriterion
{
    private readonly OrientationSet _orientations;

    public LemmingOrientationCriterion(OrientationSet orientations)
        : base(LemmingCriteria.LemmingOrientation)
    {
        _orientations = orientations;
    }

    public override bool LemmingMatchesCriteria(Lemming lemming) => _orientations.Contains(lemming.Data.Orientation);
}
