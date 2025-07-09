using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BlockerAction : LemmingAction
{
    public static readonly BlockerAction Instance = new();

    private BlockerAction()
        : base(
            LemmingActionConstants.BlockerActionId,
            LemmingActionConstants.BlockerActionName,
            LemmingActionConstants.BlockerActionSpriteFileName,
            LemmingActionConstants.BlockerAnimationFrames,
            LemmingActionConstants.MaxBlockerPhysicsFrames,
            EngineConstants.NonPermanentSkillPriority,
            LemmingActionBounds.BlockerActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemming.AnchorPosition))
            return true;

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        LevelScreen.LemmingManager.RegisterBlocker(lemming);
    }

    public static void DoBlockerCheck(Lemming lemming)
    {
        var anchorPosition = lemming.AnchorPosition;
        var footPosition = lemming.FootPosition;

        var allBlockers = LevelScreen.LemmingManager.AllBlockers;
        if (allBlockers.Count == 0)
            return;

        var influentialBlocker = GetInfluentialBlocker(
            in allBlockers,
            anchorPosition,
            footPosition,
            lemming.Orientation,
            out var forcedFacingDirection);

        if (influentialBlocker is not null)
            ForceLemmingDirection(lemming, forcedFacingDirection);
    }

    // We iterate through the blocker collection instead of using a SpacialHashGrid
    // because it is generally the case that the number of blockers in a level is small, (0 <= N < 5, etc)
    // especially compared with the number of lemmings total.

    private static Lemming? GetInfluentialBlocker(
        in LemmingEnumerable allBlockers,
        Point anchorPosition,
        Point footPosition,
        Orientation requiredOrientation,
        out FacingDirection forcedFacingDirection)
    {
        foreach (var blocker in allBlockers)
        {
            var blockerOrientation = blocker.Orientation;
            if (blockerOrientation.IsPerpendicularTo(requiredOrientation))
                continue;

            var testHitBox = GetArmHitBox(blocker, -6, -3);
            if (LevelScreen.RegionContainsEitherPoint(testHitBox, anchorPosition, footPosition))
            {
                forcedFacingDirection = requiredOrientation == blockerOrientation
                    ? FacingDirection.Left
                    : FacingDirection.Right;
                return blocker;
            }

            testHitBox = GetArmHitBox(blocker, 2, 5);
            if (LevelScreen.RegionContainsEitherPoint(testHitBox, anchorPosition, footPosition))
            {
                forcedFacingDirection = requiredOrientation == blockerOrientation
                    ? FacingDirection.Right
                    : FacingDirection.Left;
                return blocker;
            }
        }

        forcedFacingDirection = FacingDirection.Right;
        return null;
    }

    private static RectangularRegion GetArmHitBox(Lemming blocker, int offsetX0, int offsetX1)
    {
        var moveDelta = blocker.FacingDirection.Id ^ 1; // Fixes off-by-one errors between left/right
        var orientation = blocker.Orientation;
        var p0 = orientation.Move(blocker.AnchorPosition, moveDelta + offsetX0, 6);
        var p1 = orientation.Move(blocker.AnchorPosition, moveDelta + offsetX1, -4);

        return new RectangularRegion(p0, p1);
    }

    public static void ForceLemmingDirection(Lemming lemming, FacingDirection forcedFacingDirection)
    {
        if (lemming.FacingDirection == forcedFacingDirection)
            return;

        lemming.SetFacingDirection(forcedFacingDirection);

        var dx = forcedFacingDirection.DeltaX;

        var currentAction = lemming.CurrentAction;

        // Avoid moving into terrain, see http://www.lemmingsforums.net/index.php?topic=2575.0
        if (currentAction == MinerAction.Instance)
        {
            int mineDx;
            int mineDy;

            if (lemming.PhysicsFrame == 2)
            {
                mineDx = 0;
                mineDy = 0;
            }
            else if (lemming.PhysicsFrame >= 3 && lemming.PhysicsFrame < 15)
            {
                mineDx = -2 * dx;
                mineDy = -1;
            }
            else
            {
                return;
            }

            TerrainMasks.ApplyMinerMask(lemming, 1, mineDx, mineDy);

            return;
        }

        // Required for turned builders not to walk into air
        // For platformers, see http://www.lemmingsforums.net/index.php?topic=2530.0
        if ((currentAction == BuilderAction.Instance ||
             currentAction == PlatformerAction.Instance) &&
            lemming.PhysicsFrame >= 9)
        {
            BuilderAction.LayBrick(lemming);
            return;
        }

        if (currentAction != ClimberAction.Instance &&
            currentAction != SliderAction.Instance &&
            currentAction != DehoisterAction.Instance)
            return;

        // Don't move below original position
        var dy = lemming.IsStartingAction
            ? 0
            : 1;

        // Move out of the wall
        lemming.AnchorPosition = lemming.Orientation.Move(lemming.AnchorPosition, dx, dy);

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
    }
}
