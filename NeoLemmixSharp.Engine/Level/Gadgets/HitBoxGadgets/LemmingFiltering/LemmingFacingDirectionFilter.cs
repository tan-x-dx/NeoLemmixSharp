using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingFacingDirectionFilter : ILemmingFilter
{
    // Only have one facing direction to check against, since that's the only interesting case
    private readonly FacingDirection _requiredFacingDirection;

    public LemmingFacingDirectionFilter(FacingDirection requiredFacingDirection)
    {
        _requiredFacingDirection = requiredFacingDirection;
    }

    public bool MatchesLemming(Lemming lemming) => lemming.FacingDirection == _requiredFacingDirection;
}