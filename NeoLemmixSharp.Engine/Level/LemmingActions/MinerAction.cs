using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class MinerAction : LemmingAction, IDestructionAction
{
    public static MinerAction Instance { get; } = new();

    private MinerAction()
    {
    }

    public override int Id => GameConstants.MinerActionId;
    public override string LemmingActionName => "miner";
    public override int NumberOfAnimationFrames => GameConstants.MinerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var facingDirection = lemming.FacingDirection;
        var dx = facingDirection.DeltaX;

        if (lemming.AnimationFrame == 1 ||
            lemming.AnimationFrame == 2)
        {
            TerrainMasks.ApplyMinerMask(
                lemming,
                0,
                0,
                lemming.AnimationFrame - 1);
            return true;
        }

        if (lemming.AnimationFrame != 3 &&
            lemming.AnimationFrame != 15)
            return true;

        if (lemming.State.IsSlider && WalkerAction.LemmingCanDehoist(lemming, false))
        {
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        lemmingPosition = orientation.Move(lemmingPosition, dx + dx, -1);
        lemming.LevelPosition = lemmingPosition;

        if (lemming.State.IsSlider && WalkerAction.LemmingCanDehoist(lemming, true))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        // Note that all if-checks are relative to the end position!

        // Lemming cannot go down, so turn; see http://www.lemmingsforums.net/index.php?topic=2547.0
        if (TerrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.Move(lemmingPosition, -dx, -1)) &&
            TerrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.MoveDown(lemmingPosition, 1)))
        {
            var lemmingPosition0 = orientation.MoveDown(lemmingPosition, 1);
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx + dx);
            lemming.LevelPosition = lemmingPosition;
            TurnMinerAround(lemming, lemmingPosition0);
            return true;
        }

        // This first check is only relevant during the very first cycle.
        // Otherwise the pixel was already checked in frame 15 of the previous cycle
        if (lemming.AnimationFrame == 3 && TerrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.Move(lemmingPosition, -dx, 2)))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx + dx);
            lemming.LevelPosition = lemmingPosition;
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));

            return true;
        }

        // Do we really want the to check the second pixel during frame 3 ????
        if (!TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, 1)) &&
            !TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, 0)) &&
            !TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, -1)))
        {
            lemmingPosition = orientation.Move(lemmingPosition, -dx, -1);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            lemming.DistanceFallen++;
            return true;
        }

        if (TerrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.MoveDown(lemmingPosition, 2)))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));
            return true;
        }

        if (!TerrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (TerrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.Move(lemmingPosition, dx, 2)))
        {
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));

            return true;
        }

        if (!TerrainManager.PixelIsIndestructibleToLemming(lemming, this, lemmingPosition))
            return true;

        TurnMinerAround(lemming, lemmingPosition);

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -2;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;

    private static void TurnMinerAround(
        Lemming lemming,
        LevelPosition checkPosition)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (TerrainManager.PixelIsSteel(checkPosition))
        {
            // CueSoundEffect(SFX_HITS_STEEL, L.Position);
        }

        // Independently of checkPosition this check is always made at lemming's position
        // No longer check at lemming's position, due to http://www.lemmingsforums.net/index.php?topic=2547.0

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);

        if (TerrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
        {
            lemming.LevelPosition = lemmingPosition;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true); // turn around as well
        }
    }

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        var oppositeOrientationArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                            orientation.GetOpposite().RotNum;
        var oppositeOrientationArrowMask = (PixelType)(1 << oppositeOrientationArrowShift);
        if ((pixelType & oppositeOrientationArrowMask) != PixelType.Empty)
            return false;

        var facingDirectionAsOrientation = facingDirection.ConvertToRelativeOrientation(orientation);
        var oppositeFacingDirectionArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                                facingDirectionAsOrientation.GetOpposite().RotNum;
        var oppositeFacingDirectionArrowMask = (PixelType)(1 << oppositeFacingDirectionArrowShift);
        return (pixelType & oppositeFacingDirectionArrowMask) == PixelType.Empty;
    }
}
