using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class ForceFacingDirectionAction : IGadgetAction
{
    private readonly FacingDirection _facingDirection;
    public GadgetActionType ActionType => GadgetActionType.ForceFacingDirection;

    public ForceFacingDirectionAction(FacingDirection facingDirection)
    {
        _facingDirection = facingDirection;
    }

    public void PerformAction(Lemming lemming)
    {
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}