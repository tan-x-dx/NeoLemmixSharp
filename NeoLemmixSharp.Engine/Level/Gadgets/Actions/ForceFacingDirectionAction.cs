using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class ForceFacingDirectionAction : GadgetAction
{
    private readonly FacingDirection _facingDirection;

    public ForceFacingDirectionAction(FacingDirection facingDirection)
        : base(GadgetActionType.ForceFacingDirection)
    {
        _facingDirection = facingDirection;
    }

    public override void PerformAction(Lemming lemming)
    {
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}
