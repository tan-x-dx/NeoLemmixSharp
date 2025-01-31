using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class ForceFacingDirectionAction : IGadgetAction
{
    private readonly FacingDirection _facingDirection;

    public ForceFacingDirectionAction(FacingDirection facingDirection)
    {
        _facingDirection = new FacingDirection(facingDirection.Id);
    }

    public void PerformAction(Lemming lemming)
    {
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}