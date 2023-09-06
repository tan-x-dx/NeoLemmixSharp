using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FencerAction : LemmingAction, IDestructionAction
{
    public static FencerAction Instance { get; } = new();

    private FencerAction()
    {
    }

    public override int Id => GameConstants.FencerActionId;
    public override string LemmingActionName => "fencer";
    public override int NumberOfAnimationFrames => GameConstants.FencerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    public override LevelPosition GetAnchorPosition() => new(3, 10);

    protected override int TopLeftBoundsDeltaX() => -3;
    protected override int TopLeftBoundsDeltaY() => 10;

    protected override int BottomRightBoundsDeltaX() => 3;

    [Pure]
    public bool CanDestroyPixel(
        PixelType pixelType,
        Orientation orientation,
        FacingDirection facingDirection)
    {
        var orientationArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                    orientation.RotNum;
        var orientationArrowMask = (PixelType)(1 << orientationArrowShift);
        if ((pixelType & orientationArrowMask) != PixelType.Empty)
            return false;

        var facingDirectionAsOrientation = facingDirection.ConvertToRelativeOrientation(orientation);
        var oppositeFacingDirectionArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                                facingDirectionAsOrientation.GetOpposite().RotNum;
        var oppositeFacingDirectionArrowMask = (PixelType)(1 << oppositeFacingDirectionArrowShift);
        return (pixelType & oppositeFacingDirectionArrowMask) == PixelType.Empty;
    }
}