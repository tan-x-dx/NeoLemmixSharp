using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingOrientationFilter : ILemmingCriterion
{
    private readonly OrientationSet _allowedOrientations = OrientationComparer.CreateSimpleSet();

    public void RegisterOrientation(Orientation orientation)
    {
        _allowedOrientations.Add(orientation);
    }

    public bool LemmingMatchesCriteria(Lemming lemming) => _allowedOrientations.Contains(lemming.Orientation);
}