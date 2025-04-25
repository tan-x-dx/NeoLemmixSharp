using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingFacingDirectionCriterion : ILemmingCriterion
{
    private static readonly LemmingFacingDirectionCriterion MustFaceRight = new(FacingDirection.Right);
    private static readonly LemmingFacingDirectionCriterion MustFaceLeft = new(FacingDirection.Left);

    public static LemmingFacingDirectionCriterion ForFacingDirection(FacingDirection facingDirection)
    {
        return facingDirection == FacingDirection.Right
            ? MustFaceRight
            : MustFaceLeft;
    }

    // Only have one facing direction to check against, since that's the only interesting case
    private readonly FacingDirection _requiredFacingDirection;

    private LemmingFacingDirectionCriterion(FacingDirection requiredFacingDirection)
    {
        _requiredFacingDirection = requiredFacingDirection;
    }

    public bool LemmingMatchesCriteria(Lemming lemming) => lemming.FacingDirection == _requiredFacingDirection;
}