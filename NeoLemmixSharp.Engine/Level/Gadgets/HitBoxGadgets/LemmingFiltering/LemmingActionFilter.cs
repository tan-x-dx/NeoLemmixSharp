using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingActionFilter : ILemmingFilter
{
    private readonly LemmingAction? _lemmingAction;
    private readonly Orientation? _orientation;
    private readonly FacingDirection? _facingDirection;

    public LemmingActionFilter(LemmingAction? lemmingAction, Orientation? orientation, FacingDirection? facingDirection)
    {
        _lemmingAction = lemmingAction;
        _orientation = orientation;
        _facingDirection = facingDirection;
    }

    public bool MatchesLemming(Lemming lemming)
    {
        if (_lemmingAction is not null)
        {
            if (lemming.CurrentAction != _lemmingAction)
                return false;
        }

        if (_orientation is not null)
        {
            if (lemming.Orientation != _orientation)
                return false;
        }

        if (_facingDirection is not null)
        {
            if (lemming.FacingDirection != _facingDirection)
                return false;
        }

        return true;
    }
}