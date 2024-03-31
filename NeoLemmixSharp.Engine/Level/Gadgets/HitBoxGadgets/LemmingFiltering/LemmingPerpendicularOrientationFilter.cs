using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingPerpendicularOrientationFilter : ILemmingFilter
{
    private readonly Orientation _orientation;

    public LemmingPerpendicularOrientationFilter(Orientation orientation)
    {
        _orientation = orientation;
    }

    public bool MatchesLemming(Lemming lemming)
    {
        return _orientation.IsPerpendicularTo(lemming.Orientation);
    }
}