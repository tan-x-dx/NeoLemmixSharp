using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BlockerAction : LemmingAction
{
    public static BlockerAction Instance { get; } = new();

    private BlockerAction()
    {
    }

    public override int Id => Global.BlockerActionId;
    public override string LemmingActionName => "blocker";
    public override int NumberOfAnimationFrames => Global.BlockerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => Global.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (Global.TerrainManager.PixelIsSolidToLemming(lemming, lemming.LevelPosition))
            return true;

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        Global.LemmingManager.DeregisterBlocker(lemming);

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        Global.LemmingManager.RegisterBlocker(lemming);
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -7;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 11;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => -3;

    public static FacingDirection? TestBlockerMatches(
        Lemming blocker,
        Lemming lemming,
        LevelPosition anchorPosition,
        LevelPosition footPosition)
    {
        Debug.Assert(blocker.CurrentAction == Instance); // Should be a blocker
        Debug.Assert(lemming.CurrentAction != Instance); // Should NOT be a blocker

        var blockerOrientation = blocker.Orientation;
        var lemmingOrientation = lemming.Orientation;

        if (blockerOrientation.IsPerpendicularTo(lemmingOrientation))
            return null;

        var dxCorrection = 1 - blocker.FacingDirection.Id; // Fixes off-by-one errors with left/right positions
        var topLeftDelta = new LevelPosition(dxCorrection, 0);
        var bottomRightDelta = new LevelPosition(0, 0);

        var lemmingFacingDirection = lemming.FacingDirection;
        // If the lemming is upside down compared to the blocker, it cares about the opposite hands.
        if (lemmingOrientation == blockerOrientation.GetOpposite())
        {
            lemmingFacingDirection = lemmingFacingDirection.GetOpposite();
            topLeftDelta += new LevelPosition(0, 9);
            bottomRightDelta += new LevelPosition(0, 9);
        }

        if (lemmingFacingDirection == RightFacingDirection.Instance)
        {
            topLeftDelta += new LevelPosition(-5, 6);
            bottomRightDelta += new LevelPosition(-2, -3);
        }
        else
        {
            topLeftDelta += new LevelPosition(3, 6);
            bottomRightDelta += new LevelPosition(6, -3);
        }

        var blockerLevelPosition = blocker.LevelPosition;

        var p1 = blockerOrientation.MoveWithoutNormalization(blockerLevelPosition, topLeftDelta.X, topLeftDelta.Y);
        var p2 = blockerOrientation.MoveWithoutNormalization(blockerLevelPosition, bottomRightDelta.X, bottomRightDelta.Y);
        var blockingRegion = new LevelPositionPair(p1, p2);

        if (blockingRegion.Contains(anchorPosition) ||
            blockingRegion.Contains(footPosition))
            return lemming.FacingDirection.GetOpposite();

        return null;
    }

    public static void ForceLemmingDirection(Lemming lemming, FacingDirection forcedFacingDirection)
    {
        if (lemming.FacingDirection == forcedFacingDirection)
            return;

        lemming.SetFacingDirection(forcedFacingDirection);

        var dx = forcedFacingDirection.DeltaX;

        // Avoid moving into terrain, see http://www.lemmingsforums.net/index.php?topic=2575.0
        if (lemming.CurrentAction == MinerAction.Instance)
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
        if ((lemming.CurrentAction == BuilderAction.Instance ||
             lemming.CurrentAction == PlatformerAction.Instance) &&
            lemming.PhysicsFrame >= 9)
        {
            BuilderAction.LayBrick(lemming);
            return;
        }

        if (lemming.CurrentAction != ClimberAction.Instance &&
            lemming.CurrentAction != SliderAction.Instance &&
            lemming.CurrentAction != DehoisterAction.Instance)
            return;

        // Don't move below original position
        var dy = lemming.IsStartingAction
            ? 0
            : 1;

        // Move out of the wall
        lemming.LevelPosition = lemming.Orientation.Move(lemming.LevelPosition, dx, dy);

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
    }
}