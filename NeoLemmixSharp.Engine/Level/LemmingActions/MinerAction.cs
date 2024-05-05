using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class MinerAction : LemmingAction, IDestructionMask
{
    public static readonly MinerAction Instance = new();

    private MinerAction()
        : base(
            LevelConstants.MinerActionId,
            LevelConstants.MinerActionName,
            LevelConstants.MinerAnimationFrames,
            LevelConstants.MaxMinerPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority,
            false,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var facingDirection = lemming.FacingDirection;
        var dx = facingDirection.DeltaX;

        if (lemming.PhysicsFrame == 1 ||
            lemming.PhysicsFrame == 2)
        {
            TerrainMasks.ApplyMinerMask(
                lemming,
                lemming.PhysicsFrame - 1, 0, 0);
            return true;
        }

        if (lemming.PhysicsFrame != 3 &&
            lemming.PhysicsFrame != 15)
            return true;

        if (lemming.State.IsSlider && DehoisterAction.LemmingCanDehoist(lemming, false))
        {
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        lemmingPosition = orientation.Move(lemmingPosition, dx + dx, -1);

        if (lemming.State.IsSlider && DehoisterAction.LemmingCanDehoist(lemming, true))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            DehoisterAction.Instance.TransitionLemmingToAction(lemming, true);
            return true;
        }

        // Note that all if-checks are relative to the end position!

        // Lemming cannot go down, so turn; see http://www.lemmingsforums.net/index.php?topic=2547.0
        if (terrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.Move(lemmingPosition, -dx, -1)) &&
            terrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.MoveDown(lemmingPosition, 1)))
        {
            var lemmingPosition0 = orientation.MoveDown(lemmingPosition, 1);
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx + dx);
            TurnMinerAround(lemming, lemmingPosition0);
            return true;
        }

        // This first check is only relevant during the very first cycle.
        // Otherwise, the pixel was already checked in frame 15 of the previous cycle
        if (lemming.PhysicsFrame == 3 && terrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.Move(lemmingPosition, -dx, 2)))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx + dx);
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));

            return true;
        }

        // Do we really want the to check the second pixel during frame 3 ????
        if (!terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, 1)) &&
            !terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, 0)) &&
            !terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, -1)))
        {
            lemmingPosition = orientation.Move(lemmingPosition, -dx, -1);
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            lemming.DistanceFallen++;
            return true;
        }

        if (terrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.MoveDown(lemmingPosition, 2)))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));
            return true;
        }

        if (!terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (terrainManager.PixelIsIndestructibleToLemming(lemming, this, orientation.Move(lemmingPosition, dx, 2)))
        {
            TurnMinerAround(lemming, orientation.Move(lemmingPosition, dx, 2));

            return true;
        }

        if (!terrainManager.PixelIsIndestructibleToLemming(lemming, this, lemmingPosition))
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
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (terrainManager.PixelIsSteel(checkPosition))
        {
            // CueSoundEffect(SFX_HITS_STEEL, L.Position);
        }

        // Independently of checkPosition this check is always made at lemming's position
        // No longer check at lemming's position, due to http://www.lemmingsforums.net/index.php?topic=2547.0

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);

        if (terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
        {
            lemming.LevelPosition = lemmingPosition;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true); // turn around as well
        }
    }

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        var oppositeOrientationArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                            Orientation.GetOpposite(orientation).RotNum;
        var oppositeOrientationArrowMask = (PixelType)(1 << oppositeOrientationArrowShift);
        if ((pixelType & oppositeOrientationArrowMask) != PixelType.Empty)
            return false;

        var facingDirectionAsOrientation = facingDirection.ConvertToRelativeOrientation(orientation);
        var oppositeFacingDirectionArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                                Orientation.GetOpposite(facingDirectionAsOrientation).RotNum;
        var oppositeFacingDirectionArrowMask = (PixelType)(1 << oppositeFacingDirectionArrowShift);
        return (pixelType & oppositeFacingDirectionArrowMask) == PixelType.Empty;
    }
}
