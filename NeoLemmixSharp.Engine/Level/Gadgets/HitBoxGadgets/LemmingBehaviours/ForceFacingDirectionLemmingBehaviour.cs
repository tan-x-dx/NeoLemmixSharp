using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.LemmingActions;

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

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var lemming = GetLemming(lemmingId);
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}
