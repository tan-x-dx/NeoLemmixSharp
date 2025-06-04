using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class ForceFacingDirectionAction : GadgetAction
{
    private static readonly ForceFacingDirectionAction FaceRight = new(FacingDirection.Right);
    private static readonly ForceFacingDirectionAction FaceLeft = new(FacingDirection.Left);

    public static ForceFacingDirectionAction ForFacingDirection(int facingDirectionId)
    {
        var facingDirection = new FacingDirection(facingDirectionId);
        return facingDirection == FacingDirection.Right
            ? FaceRight
            : FaceLeft;
    }

    private readonly FacingDirection _facingDirection;

    private ForceFacingDirectionAction(FacingDirection facingDirection)
        : base(GadgetActionType.ForceFacingDirection)
    {
        _facingDirection = facingDirection;
    }

    public override void PerformAction(Lemming lemming)
    {
        BlockerAction.ForceLemmingDirection(lemming, _facingDirection);
    }
}
