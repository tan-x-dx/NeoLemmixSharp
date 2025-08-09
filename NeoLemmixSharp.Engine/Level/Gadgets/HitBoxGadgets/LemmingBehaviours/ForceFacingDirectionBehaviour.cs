using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
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

    public override void PerformBehaviour(Lemming lemming)
    {
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}
