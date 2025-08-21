using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class ForceFacingDirectionBehaviour : LemmingBehaviour
{
    private readonly FacingDirection _facingDirection;

    public ForceFacingDirectionBehaviour(
        FacingDirection facingDirection)
        : base(LemmingBehaviourType.ForceFacingDirection)
    {
        _facingDirection = facingDirection;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var lemming = GetLemming(lemmingId);
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}
