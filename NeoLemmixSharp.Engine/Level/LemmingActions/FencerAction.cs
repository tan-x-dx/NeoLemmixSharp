using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FencerAction : LemmingAction, IDestructionMask
{
    public static readonly FencerAction Instance = new();

    private FencerAction()
    {
    }

    public override int Id => LevelConstants.FencerActionId;
    public override string LemmingActionName => "fencer";
    public override int NumberOfAnimationFrames => LevelConstants.FencerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

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