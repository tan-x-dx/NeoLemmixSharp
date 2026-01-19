using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class ForceFacingDirectionLemmingBehaviour : LemmingBehaviour
{
    private readonly FacingDirection _facingDirection;

    public ForceFacingDirectionLemmingBehaviour(
        FacingDirection facingDirection)
        : base(LemmingBehaviourType.ForceLemmingFacingDirection)
    {
        _facingDirection = facingDirection;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}
