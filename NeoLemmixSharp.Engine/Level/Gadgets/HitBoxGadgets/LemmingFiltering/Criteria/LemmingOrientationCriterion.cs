using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;

public sealed class LemmingOrientationCriterion : LemmingCriterion
{
    private readonly OrientationSet _orientations;

    public LemmingOrientationCriterion(OrientationSet orientations)
        : base(LemmingCriteriaType.LemmingOrientation)
    {
        _orientations = orientations;
    }

    public override bool LemmingMatchesCriteria(Lemming lemming) => _orientations.Contains(lemming.Orientation);
}
