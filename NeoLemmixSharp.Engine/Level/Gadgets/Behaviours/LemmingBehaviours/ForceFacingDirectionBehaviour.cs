using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.LemmingBehaviours;

public sealed class ForceFacingDirectionBehaviour : LemmingBehaviour
{
    private static readonly ForceFacingDirectionBehaviour FaceRight = new(FacingDirection.Right);
    private static readonly ForceFacingDirectionBehaviour FaceLeft = new(FacingDirection.Left);

    public static ForceFacingDirectionBehaviour ForFacingDirection(int facingDirectionId)
    {
        var facingDirection = new FacingDirection(facingDirectionId);
        return facingDirection == FacingDirection.Right
            ? FaceRight
            : FaceLeft;
    }

    private readonly FacingDirection _facingDirection;

    private ForceFacingDirectionBehaviour(FacingDirection facingDirection)
        : base(LemmingBehaviourType.ForceFacingDirection)
    {
        _facingDirection = facingDirection;
    }

    public override void PerformBehaviour(Lemming lemming)
    {
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}
