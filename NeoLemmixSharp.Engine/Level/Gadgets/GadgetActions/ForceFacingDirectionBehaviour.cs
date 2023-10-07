using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public sealed class ForceFacingDirectionBehaviour : IGadgetBehaviour
{
    private readonly FacingDirection _facingDirection;

    public ForceFacingDirectionBehaviour(FacingDirection facingDirection)
    {
        _facingDirection = facingDirection;
    }

    public void PerformAction(Lemming lemming)
    {
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}