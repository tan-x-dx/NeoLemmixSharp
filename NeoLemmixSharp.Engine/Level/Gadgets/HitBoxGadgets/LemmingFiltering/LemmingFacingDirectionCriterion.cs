using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingFacingDirectionCriterion : ILemmingCriterion
{
    private static readonly LemmingFacingDirectionCriterion MustFaceRight = new(EngineConstants.RightFacingDirectionId);
    private static readonly LemmingFacingDirectionCriterion MustFaceLeft = new(EngineConstants.LeftFacingDirectionId);

    public static LemmingFacingDirectionCriterion ForFacingDirection(FacingDirection facingDirection)
    {
        if (facingDirection == FacingDirection.Right)
            return MustFaceRight;
        return MustFaceLeft;
    }

    // Only have one facing direction to check against, since that's the only interesting case
    private readonly int _requiredFacingDirectionId;

    private LemmingFacingDirectionCriterion(int requiredFacingDirectionId)
    {
        _requiredFacingDirectionId = requiredFacingDirectionId;
    }

    public bool LemmingMatchesCriteria(Lemming lemming) => lemming.FacingDirection.Id == _requiredFacingDirectionId;
}