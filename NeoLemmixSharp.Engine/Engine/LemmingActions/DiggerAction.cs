using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Engine.Terrain;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.LemmingActions;

public sealed class DiggerAction : LemmingAction, IDestructionAction
{
    public static DiggerAction Instance { get; } = new();

    private DiggerAction()
    {
    }

    public override int Id => GameConstants.DiggerActionId;
    public override string LemmingActionName => "digger";
    public override int NumberOfAnimationFrames => GameConstants.DiggerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        var lemmingPosition = lemming.LevelPosition;

        if (lemming.IsStartingAction)
        {
            lemming.IsStartingAction = false;
            DigOneRow(lemming, orientation, facingDirection, orientation.MoveUp(lemmingPosition, 1));
            // The first digger cycle is one frame longer!
            // So we need to artificially cancel the very first frame advancement.
            lemming.AnimationFrame--;
        }

        if (lemming.AnimationFrame != 0 &&
            lemming.AnimationFrame != 8)
            return true;

        var continueDigging = DigOneRow(lemming, orientation, facingDirection, lemmingPosition);

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        lemming.LevelPosition = lemmingPosition;

        if (Terrain.PixelIsIndestructibleToLemming(lemming, this, lemmingPosition))
        {
            if (Terrain.PixelIsSteel(lemmingPosition))
            {
                //CueSoundEffect(SFX_HITS_STEEL, L.Position);
            }

            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (continueDigging)
            return true;

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    private bool DigOneRow(
        Lemming lemming,
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition lemmingPosition)
    {
        // The central pixel of the removed row lies at the lemming's position
        var result = false;

        // Two most extreme pixels
        var checkLevelPosition = orientation.Move(lemmingPosition, -4, 0);
        var pixelIsSolid = Terrain.PixelIsSolidToLemming(lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            Terrain.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
        }

        checkLevelPosition = orientation.Move(lemmingPosition, 4, 0);
        pixelIsSolid = Terrain.PixelIsSolidToLemming(lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            Terrain.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
        }

        // Everything in between
        for (var i = -3; i < 4; i++)
        {
            checkLevelPosition = orientation.Move(lemmingPosition, i, 0);
            pixelIsSolid = Terrain.PixelIsSolidToLemming(lemming, checkLevelPosition);
            if (pixelIsSolid)
            {
                Terrain.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
                result = true;
            }
        }

        // Delete these pixels from the terrain layer
        // ?? if not IsSimulating then fRenderInterface.RemoveTerrain(PosX - 4, PosY, 9, 1); ??
        return result;
    }

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        var oppositeArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                 orientation.GetOpposite().RotNum;
        var oppositeArrowMask = (PixelType)(1 << oppositeArrowShift);
        return (pixelType & oppositeArrowMask) == PixelType.Empty;
    }
}
